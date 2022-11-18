﻿// <copyright file="HttpRequestProcessor.cs" company="On Test Automation">
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

namespace RestAssured.Net.RA.Internal
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using RestAssuredNet.RA;
    using RestAssuredNet.RA.Exceptions;

    /// <summary>
    /// The <see cref="HttpRequestProcessor"/> class is responsible for sending HTTP requests.
    /// </summary>
    public class HttpRequestProcessor : IDisposable
    {
        private readonly HttpClientHandler handler;
        private readonly HttpClient client;
        private CookieContainer cookieContainer = new CookieContainer();
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestProcessor"/> class.
        /// </summary>
        /// <param name="proxy">The <see cref="IWebProxy"/> to set on the <see cref="HttpClientHandler"/> used with the <see cref="HttpClient"/>.</param>
        /// <param name="useRelaxedHttpsValidation">If set to true, SSL check is disabled.</param>
        public HttpRequestProcessor(IWebProxy? proxy, bool useRelaxedHttpsValidation)
        {
            this.handler = new HttpClientHandler
            {
                CookieContainer = this.cookieContainer,
                Proxy = proxy,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => useRelaxedHttpsValidation,
            };
            this.client = new HttpClient(this.handler);
        }

        /// <summary>
        /// Sets the timeout on the HTTP client to the specified value.
        /// </summary>
        /// <param name="timeout">The timeout to set on the HTTP client.</param>
        public void SetTimeout(TimeSpan timeout)
        {
            this.client.Timeout = timeout;
        }

        /// <summary>
        /// Sends an HTTP request message object and returns the response.
        /// </summary>
        /// <param name="request">The HTTP request message object to be sent.</param>
        /// <param name="cookieCollection">The <see cref="CookieCollection"/> to add to the request before it is sent.</param>
        /// <returns>The HTTP response.</returns>
        /// <exception cref="HttpRequestProcessorException">Thrown whenever the HTTP request fails.</exception>
        public async Task<VerifiableResponse> Send(HttpRequestMessage request, CookieCollection cookieCollection)
        {
            foreach (Cookie cookie in cookieCollection)
            {
                // The domain for a cookie cannot be empty, so set it to the hostname for
                // the request if it has not been set already
                if (cookie.Domain == null || cookie.Domain == string.Empty)
                {
                    cookie.Domain = request.RequestUri.Host;
                }
            }

            this.cookieContainer.Add(cookieCollection);

            try
            {
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                HttpResponseMessage response = await this.client.SendAsync(request);
                stopwatch.Stop();

                return new VerifiableResponse(response, stopwatch.Elapsed);
            }
            catch (HttpRequestException hre)
            {
                throw new HttpRequestProcessorException(hre.Message, hre);
            }
        }

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
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.client.Dispose();
            this.handler.Dispose();
            this.disposed = true;
        }
    }
}