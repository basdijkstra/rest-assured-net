// <copyright file="HttpRequestProcessor.cs" company="On Test Automation">
// Copyright 2019 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
namespace RestAssured.Request
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using RestAssured.Request.Cookies;
    using RestAssured.Request.Exceptions;
    using RestAssured.Response;

    /// <summary>
    /// The <see cref="HttpRequestProcessor"/> class is responsible for sending HTTP requests.
    /// </summary>
    internal class HttpRequestProcessor : IDisposable
    {
        /// <summary>
        /// The timeout used when no timeout was set and the <see cref="HttpClient"/> was created by this class.
        /// This is equal to the default timeout of an <see cref="HttpClient"/>.
        /// </summary>
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100);

        private readonly HttpClientHandler httpClientHandler;
        private readonly HttpClient httpClient;
        private readonly bool isSuppliedHttpClient;
        private CookieContainer cookieContainer = new CookieContainer();
        private bool disposed = false;

        /// <summary>
        /// Implements Dispose() method of IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            this.Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implements Dispose(bool) method of IDisposable interface.
        /// </summary>
        /// <param name="disposing">Flag indicating whether objects should be disposed.</param>
        public virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.httpClient.Dispose();
            this.httpClientHandler.Dispose();
            this.disposed = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestProcessor"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use when sending requests.</param>
        /// <param name="proxy">The <see cref="IWebProxy"/> to set on the <see cref="HttpClientHandler"/> used with the <see cref="HttpClient"/>.</param>
        /// <param name="disableSslCertificateValidation">If set to true, SSL certificate validation is disabled.</param>
        /// <param name="networkCredential">The <see cref="NetworkCredential"/> to add to the <see cref="HttpClientHandler"/> to use in the request.</param>
        internal HttpRequestProcessor(HttpClient? httpClient, IWebProxy? proxy, bool disableSslCertificateValidation, NetworkCredential networkCredential)
        {
            if (disableSslCertificateValidation)
            {
                this.httpClientHandler = new HttpClientHandler
                {
                    CookieContainer = this.cookieContainer,
                    Proxy = proxy,
                    Credentials = networkCredential,
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
                };
            }
            else
            {
                this.httpClientHandler = new HttpClientHandler
                {
                    CookieContainer = this.cookieContainer,
                    Proxy = proxy,
                    Credentials = networkCredential,
                    ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation,
                };
            }

            if (httpClient != null)
            {
                this.httpClient = httpClient;
                this.isSuppliedHttpClient = true;
            }
            else
            {
                // Timeouts are applied per request using a cancellation token (see Send()),
                // so the timeout of the HttpClient itself must not cut off longer timeouts.
                this.httpClient = new HttpClient(this.httpClientHandler)
                {
                    Timeout = System.Threading.Timeout.InfiniteTimeSpan,
                };
            }
        }

        /// <summary>
        /// Sends an HTTP request message object and returns the response.
        /// </summary>
        /// <param name="request">The HTTP request message object to be sent.</param>
        /// <param name="cookieCollection">The <see cref="CookieCollection"/> to add to the request before it is sent.</param>
        /// <param name="httpCompletionOption">The <see cref="HttpCompletionOption"/> setting to use when sending the request.</param>
        /// <param name="timeout">The timeout for this request, or <c>null</c> if no timeout was set in the request or request specification.</param>
        /// <returns>The HTTP response.</returns>
        /// <exception cref="HttpRequestProcessorException">Thrown whenever the HTTP request fails.</exception>
        /// <exception cref="TimeoutException">Thrown whenever the request times out, with the timeout that was exceeded in the message.</exception>
        internal async Task<VerifiableResponse> Send(HttpRequestMessage request, CookieCollection cookieCollection, HttpCompletionOption httpCompletionOption, TimeSpan? timeout)
        {
            // The timeout is applied using a cancellation token, so that the HttpClient itself is never changed.
            // An HttpClient supplied by the user keeps its own timeout as well: the shorter of the two applies.
            TimeSpan? requestTimeout = timeout ?? (this.isSuppliedHttpClient ? null : DefaultTimeout);
            using CancellationTokenSource? timeoutTokenSource = requestTimeout != null ? new CancellationTokenSource((TimeSpan)requestTimeout) : null;

            CookieUtils cookieUtils = new CookieUtils();

            foreach (Cookie cookie in cookieCollection)
            {
                cookieUtils.SetDomainFor(cookie, request.RequestUri!.Host);
            }

            this.cookieContainer.Add(cookieCollection);

            // A user-supplied HttpClient does not use the handler (and the cookie container) created by this class,
            // so the request cookies are added to the request as a Cookie header instead.
            if (this.isSuppliedHttpClient)
            {
                this.AddCookieHeader(request);
            }

            try
            {
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                HttpResponseMessage response = await this.httpClient.SendAsync(request, httpCompletionOption, timeoutTokenSource?.Token ?? CancellationToken.None);
                stopwatch.Stop();

                CookieContainer responseCookies = cookieUtils.GetResponseCookies(this.cookieContainer, cookieCollection, response);

                return new VerifiableResponse(response, responseCookies, stopwatch.Elapsed);
            }
            catch (HttpRequestException hre)
            {
                throw new HttpRequestProcessorException(hre.Message, hre);
            }
            catch (TaskCanceledException tce)
            {
                // Report the timeout that was actually exceeded: either the one set for this request,
                // or the timeout of the HttpClient (when the user supplied their own HttpClient).
                TimeSpan exceededTimeout = timeoutTokenSource?.IsCancellationRequested == true ? (TimeSpan)requestTimeout! : this.httpClient.Timeout;
                throw new TimeoutException($"Request timeout of {exceededTimeout} exceeded.", tce);
            }
        }

        private static bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors sslErrors)
        {
            return sslErrors == SslPolicyErrors.None;
        }

        /// <summary>
        /// Adds the request cookies that apply to the request URI to the Cookie header of the request.
        /// The cookie container applies the same rules (domain, path, secure, expiry) as when the cookies are sent by the handler.
        /// A Cookie header that was already set in the request is kept, and combined with the request cookies into a single header.
        /// </summary>
        /// <param name="request">The request to add the Cookie header to.</param>
        private void AddCookieHeader(HttpRequestMessage request)
        {
            string requestCookies = this.cookieContainer.GetCookieHeader(request.RequestUri!);

            if (requestCookies.Length == 0)
            {
                return;
            }

            if (request.Headers.TryGetValues("Cookie", out var existingValues))
            {
                requestCookies = string.Join("; ", existingValues.Append(requestCookies));
                request.Headers.Remove("Cookie");
            }

            request.Headers.TryAddWithoutValidation("Cookie", requestCookies);
        }
    }
}