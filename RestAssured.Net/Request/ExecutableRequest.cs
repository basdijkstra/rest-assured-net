// <copyright file="ExecutableRequest.cs" company="On Test Automation">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Newtonsoft.Json;
    using RestAssured.Configuration;
    using RestAssured.Logging;
    using RestAssured.Request.Builders;
    using RestAssured.Request.Exceptions;
    using RestAssured.Response;

    /// <summary>
    /// The request to be sent.
    /// </summary>
    public class ExecutableRequest : IDisposable
    {
        private readonly HttpClient? httpClient;
        private HttpRequestMessage request = new HttpRequestMessage();
        private CookieCollection cookieCollection = new CookieCollection();
        private RequestSpecification? requestSpecification;
        private object requestBody = string.Empty;
        private string contentTypeHeader = "application/json";
        private Encoding contentEncoding = Encoding.UTF8;
        private IEnumerable<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
        private Dictionary<string, string> pathParams = new Dictionary<string, string>();
        private IEnumerable<KeyValuePair<string, string>>? formData = null;
        private MultipartFormDataContent? multipartFormDataContent = null;
        private TimeSpan? timeout = null;
        private IWebProxy? proxy = null;
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        private List<string> sensitiveRequestHeadersAndCookies = new List<string>();
        private HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead;
        private NetworkCredential? networkCredential = null;
        private bool disableSslCertificateValidation = false;
        private bool stripCharset = false;
        private bool disposed = false;

        /// <summary>
        /// The request logging level for this request.
        /// </summary>
        internal Logging.RequestLogLevel RequestLoggingLevel { get; set; }

        /// <summary>
        /// The response logging level for this request.
        /// </summary>
        internal Response.Logging.ResponseLogLevel ResponseLoggingLevel { get; set; }

        /// <summary>
        /// The configuration settings to use when logging request and response details.
        /// </summary>
        internal LogConfiguration? LogConfiguration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableRequest"/> class.
        /// </summary>
        /// <param name="config">The <see cref="RestAssuredConfiguration"/> to use for all requests.</param>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use when sending requests.</param>
        internal ExecutableRequest(RestAssuredConfiguration config, HttpClient? httpClient)
        {
            this.disableSslCertificateValidation = config.DisableSslCertificateValidation;

            this.RequestLoggingLevel = config.RequestLogLevel;
            this.ResponseLoggingLevel = config.ResponseLogLevel;
            this.LogConfiguration = config.LogConfiguration;
            this.httpCompletionOption = config.HttpCompletionOption;

            this.httpClient = httpClient;
        }

        /// <summary>
        /// Adds a <see cref="RequestSpecification"/> to the request properties.
        /// </summary>
        /// <param name="requestSpecification">The <see cref="RequestSpecification"/> to use when building the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest Spec(RequestSpecification requestSpecification)
        {
            this.requestSpecification = requestSpecification;
            this.RequestLoggingLevel = requestSpecification.RequestLogLevel;
            return this;
        }

        /// <summary>
        /// Adds a request header and the associated value to the request object to be sent.
        /// </summary>
        /// <param name="key">The header key that is to be added to the request.</param>
        /// <param name="value">The associated header value that is to be added to the request.</param>
        /// <param name="validate">Boolean indicating whether the header value should be validated before being added to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest Header(string key, object value, bool validate = true)
        {
            if (validate)
            {
                this.request.Headers.Add(key, value.ToString());
            }
            else
            {
                this.request.Headers.TryAddWithoutValidation(key, value.ToString());
            }

            return this;
        }

        /// <summary>
        /// Adds a request header and the associated values to the request object to be sent.
        /// </summary>
        /// <param name="key">The header key that is to be added to the request.</param>
        /// <param name="values">The associated header values that are to be added to the request.</param>
        /// /// <param name="validate">Boolean indicating whether the header value should be validated before being added to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest Header(string key, IEnumerable<string> values, bool validate = true)
        {
            if (validate)
            {
                this.request.Headers.Add(key, values);
            }
            else
            {
                this.request.Headers?.TryAddWithoutValidation(key, values);
            }

            return this;
        }

        /// <summary>
        /// Adds a <see cref="IDictionary{TKey, TValue}"/> of request headers and their associated values to the request object to be sent.
        /// </summary>
        /// <param name="headers">An <see cref="Dictionary{TKey, TValue}"/> containing the headers to be added to the request.</param>
        /// <param name="validate">Boolean indicating whether the header value should be validated before being added to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest Headers(Dictionary<string, object> headers, bool validate = true)
        {
            headers.ToList().ForEach(header =>
            {
                if (validate)
                {
                    this.request.Headers.Add(header.Key, header.Value.ToString());
                }
                else
                {
                    this.request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToString());
                }
            });

            return this;
        }

        /// <summary>
        /// Adds a Content-Type header and the specified value to the request object to be sent.
        /// </summary>
        /// <param name="contentType">The value for the Content-Type header to be added.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest ContentType(string contentType)
        {
            this.contentTypeHeader = contentType;
            return this;
        }

        /// <summary>
        /// Sets the content character encoding for the request object to be sent.
        /// </summary>
        /// <param name="encoding">The value for the character encoding to be added.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest ContentEncoding(Encoding encoding)
        {
            this.contentEncoding = encoding;
            return this;
        }

        /// <summary>
        /// Sets the value for the Accept header for the request object to be sent.
        /// </summary>
        /// <param name="accept">The value for the Accept header to be added.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest Accept(string accept)
        {
            this.request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
            return this;
        }

        /// <summary>
        /// Adds the specified query parameter with all of its values to the endpoint when the request is sent.
        /// </summary>
        /// <param name="key">The query parameter name.</param>
        /// <param name="values">The associated query parameter values.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest QueryParam(string key, params object[] values)
        {
            foreach (object value in values)
            {
                this.queryParams = this.queryParams.Append(new KeyValuePair<string, string>(key, value.ToString() ?? string.Empty));
            }

            return this;
        }

        /// <summary>
        /// Adds the specified query parameters to the endpoint when the request is sent.
        /// </summary>
        /// <param name="queryParams">A <see cref="Dictionary{TKey, TValue}"/> containing the query parameters to be added.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        [Obsolete("Please use QueryParams(IEnumerable<KeyValuePair<string, object>>) instead. This method will be removed in version 5.0.0")]
        public ExecutableRequest QueryParams(Dictionary<string, object> queryParams)
        {
            return this.QueryParams((IEnumerable<KeyValuePair<string, object>>)queryParams);
        }

        /// <summary>
        /// Adds the specified query parameters to the endpoint when the request is sent.
        /// </summary>
        /// <param name="queryParams">A <see cref="IEnumerable{T}"/> containing the query parameters to be added.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest QueryParams(IEnumerable<KeyValuePair<string, object>> queryParams)
        {
            queryParams.ToList().ForEach(param => this.queryParams = this.queryParams.Append(new KeyValuePair<string, string>(param.Key, string.Join(",", param.Value))));
            return this;
        }

        /// <summary>
        /// Adds a path parameter to the endpoint when the request is sent.
        /// </summary>
        /// <param name="key">The path parameter name.</param>
        /// <param name="value">The associated path parameter value.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest PathParam(string key, object value)
        {
            this.pathParams[key] = value.ToString() ?? string.Empty;
            return this;
        }

        /// <summary>
        /// Adds the specified path parameters to the endpoint when the request is sent.
        /// </summary>
        /// <param name="pathParams">A <see cref="Dictionary{TKey, TValue}"/> containing the path parameters to be added.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest PathParams(Dictionary<string, object> pathParams)
        {
            pathParams.ToList().ForEach(param => this.pathParams[param.Key] = param.Value.ToString() ?? string.Empty);
            return this;
        }

        /// <summary>
        /// Adds a basic authentication header to the request.
        /// </summary>
        /// <param name="username">The username to be used for authentication.</param>
        /// <param name="password">The password to be used for authentication.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest BasicAuth(string username, string password)
        {
            string base64EncodedBasicAuthDetails = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            this.request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedBasicAuthDetails);
            return this;
        }

        /// <summary>
        /// Adds an OAuth2 authentication token to the request.
        /// </summary>
        /// <param name="token">The OAuth2 authentication token to be added to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest OAuth2(string token)
        {
            this.request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return this;
        }

        /// <summary>
        /// Adds NTLM authentication to the request using cached default network credentials.
        /// </summary>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest NtlmAuth()
        {
            this.networkCredential = CredentialCache.DefaultNetworkCredentials;
            return this;
        }

        /// <summary>
        /// Adds NTLM authentication to the request using specified NTLM credentials.
        /// </summary>
        /// <param name="username">The username to use when authenticating via NTLM.</param>
        /// <param name="password">The password to use when authenticating via NTLM.</param>
        /// <param name="domain">The domain to use when authenticating via NTLM.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest NtlmAuth(string username = "", string password = "", string domain = "")
        {
            this.networkCredential = new NetworkCredential(username, password, domain);
            return this;
        }

        /// <summary>
        /// Adds a cookie to the request.
        /// </summary>
        /// <param name="cookieName">The cookie name to add to the request.</param>
        /// <param name="cookieValue">The associated cookie value to add to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest Cookie(string cookieName, string cookieValue)
        {
            this.cookieCollection.Add(new Cookie(cookieName, cookieValue));
            return this;
        }

        /// <summary>
        /// Adds a <see cref="Cookie(System.Net.Cookie)"/> to the request.
        /// </summary>
        /// <param name="cookie">The cookie to add to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest Cookie(Cookie cookie)
        {
            this.cookieCollection.Add(cookie);
            return this;
        }

        /// <summary>
        /// Adds a <see cref="CookieCollection"/> to the request.
        /// </summary>
        /// <param name="cookieCollection">The <see cref="CookieCollection"/> to add to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest Cookie(CookieCollection cookieCollection)
        {
            this.cookieCollection.Add(cookieCollection);
            return this;
        }

        /// <summary>
        /// Adds form data (x-www-form-urlencoded) to the request.
        /// </summary>
        /// <param name="formData">The form data to add to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest FormData(IEnumerable<KeyValuePair<string, string>> formData)
        {
            this.formData = formData;
            return this;
        }

        /// <summary>
        /// Adds multipart form data (multipart/form-data) to the request. Useful for file uploads.
        /// </summary>
        /// /// <param name="fileName">The path to the file that is to be uploaded with the request.</param>
        /// <param name="controlName">The control name associated with the multipart file to be uploaded.</param>
        /// <param name="contentType">The content type to be associated with the multipart file. Will be automatically determined if not specified.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest MultiPart(FileInfo fileName, string controlName = "file", MediaTypeHeaderValue? contentType = null)
        {
            this.multipartFormDataContent ??= new MultipartFormDataContent();

            try
            {
                StreamContent fileContents = new StreamContent(fileName.OpenRead());
                fileContents.Headers.ContentType = contentType ??= RequestBodyFactory.GetContentTypeForFile(fileName);

                this.multipartFormDataContent.Add(fileContents, controlName, fileName.Name);
            }
            catch (IOException ioe)
            {
                throw new RequestCreationException(ioe.Message);
            }

            return this;
        }

        /// <summary>
        /// Adds multipart form data (multipart/form-data) to the request.
        /// </summary>
        /// <param name="name">The name associated with the <see cref="HttpContent"/> in the request.</param>
        /// <param name="content">The <see cref="HttpContent"/> to be uploaded with the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest MultiPart(string name, HttpContent content)
        {
            this.multipartFormDataContent ??= new MultipartFormDataContent();

            this.multipartFormDataContent.Add(content, name);

            return this;
        }

        /// <summary>
        /// Adds multipart form data (multipart/form-data) to the request.
        /// </summary>
        /// <param name="content">A <see cref="Dictionary{TKey, TValue}"/> containing the <see cref="HttpContent"/> to be uploaded with the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest MultiPart(Dictionary<string, HttpContent> content)
        {
            this.multipartFormDataContent ??= new MultipartFormDataContent();

            content.ToList().ForEach(entry => this.multipartFormDataContent.Add(entry.Value, entry.Key));

            return this;
        }

        /// <summary>
        /// Forms a GraphQL request to be POSTed to a GraphQL API endpoint.
        /// </summary>
        /// <param name="graphQLRequest">The <see cref="GraphQLRequest"/> object to use in constructing the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest GraphQL(GraphQLRequest graphQLRequest)
        {
            this.requestBody = new
            {
                query = graphQLRequest.Query,
                operationName = graphQLRequest.OperationName,
                variables = graphQLRequest.Variables,
            };
            return this;
        }

        /// <summary>
        /// Sets a custom timeout for the request.
        /// </summary>
        /// <param name="timeout">The duration of the custom timeout as a <see cref="TimeSpan"/>.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest Timeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets a custom User Agent value for the request.
        /// </summary>
        /// <param name="product">The <see cref="ProductInfoHeaderValue"/> for the user agent to add to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest UserAgent(ProductInfoHeaderValue product)
        {
            this.request.Headers.UserAgent.Add(product);
            return this;
        }

        /// <summary>
        /// Sets a custom User Agent value for the request.
        /// </summary>
        /// <param name="productName">The value for the user agent product name to add to the request.</param>
        /// <param name="productVersion">the value for the user agent product version to add to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest UserAgent(string productName, string productVersion)
        {
            this.UserAgent(new ProductInfoHeaderValue(productName, productVersion));
            return this;
        }

        /// <summary>
        /// Sets the proxy to add to the <see cref="HttpClientHandler"/> used for this request.
        /// </summary>
        /// <param name="proxy">The <see cref="IWebProxy"/> to add to the <see cref="HttpClientHandler"/>.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest Proxy(IWebProxy proxy)
        {
            this.proxy = proxy;
            return this;
        }

        /// <summary>
        /// Disables validation of SSL certificates for this request.
        /// </summary>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest DisableSslCertificateValidation()
        {
            this.disableSslCertificateValidation = true;
            return this;
        }

        /// <summary>
        /// Sets the JSON serializer settings to be used when serializing request payloads to JSON.
        /// </summary>
        /// <param name="jsonSerializerSettings">The <see cref="JsonSerializerSettings"/> to use when serializing request payloads to JSON.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest JsonSerializerSettings(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings;
            return this;
        }

        /// <summary>
        /// Sets the configuration for logging request and response details to the specified values.
        /// </summary>
        /// <param name="logConfiguration">The log configuration settings to use.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest Log(LogConfiguration logConfiguration)
        {
            this.LogConfiguration = logConfiguration;
            return this;
        }

        /// <summary>
        /// Logs request details to the standard output.
        /// </summary>
        /// <param name="requestLogLevel">The desired request log level.</param>
        /// <param name="sensitiveHeaderOrCookieNames">The names of the request headers or cookies to be masked when logging.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        [Obsolete("Use Log(LogConfiguration logConfiguration) instead. This method will be removed in RestAssured.Net 5.0.0")]
        public ExecutableRequest Log(Logging.RequestLogLevel requestLogLevel, List<string>? sensitiveHeaderOrCookieNames = null)
        {
            this.RequestLoggingLevel = requestLogLevel;

            if (sensitiveHeaderOrCookieNames != null)
            {
                this.sensitiveRequestHeadersAndCookies.AddRange(sensitiveHeaderOrCookieNames);
            }

            return this;
        }

        /// <summary>
        /// Adds a request body to the request object to be sent.
        /// </summary>
        /// <param name="body">The body that is to be sent with the request.</param>
        /// <param name="stripCharset">Flag indicating whether the body should be sent without a specific encoding indicator.</param>
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest Body(object body, bool stripCharset = false)
        {
            this.requestBody = body;
            this.stripCharset = stripCharset;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="HttpCompletionOption"/> value to be used when sending the request.
        /// See https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcompletionoption for more details.
        /// </summary>
        /// <param name="httpCompletionOption">The <see cref="HttpCompletionOption"/> value to use in this request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest UseHttpCompletionOption(HttpCompletionOption httpCompletionOption)
        {
            this.httpCompletionOption = httpCompletionOption;
            return this;
        }

        /// <summary>
        /// Syntactic sugar that makes tests read more like natural language.
        /// </summary>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public ExecutableRequest And()
        {
            return this;
        }

        /// <summary>
        /// Syntactic sugar (for now) to help indicate the start of the 'Act' part of a test.
        /// </summary>
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest When()
        {
            return this;
        }

        /// <summary>
        /// Performs an HTTP GET.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP GET request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Get(string endpoint)
        {
            return this.Send(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Performs an HTTP POST.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP POST request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Post(string endpoint)
        {
            return this.Send(HttpMethod.Post, endpoint);
        }

        /// <summary>
        /// Performs an HTTP PUT.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP PUT request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Put(string endpoint)
        {
            return this.Send(HttpMethod.Put, endpoint);
        }

        /// <summary>
        /// Performs an HTTP PATCH.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP PATCH request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Patch(string endpoint)
        {
            return this.Send(HttpMethod.Patch, endpoint);
        }

        /// <summary>
        /// Performs an HTTP DELETE.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP DELETE request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Delete(string endpoint)
        {
            return this.Send(HttpMethod.Delete, endpoint);
        }

        /// <summary>
        /// Performs an HTTP HEAD.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP HEAD request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Head(string endpoint)
        {
            return this.Send(HttpMethod.Head, endpoint);
        }

        /// <summary>
        /// Performs an HTTP OPTIONS.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP OPTIONS request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Options(string endpoint)
        {
            return this.Send(HttpMethod.Options, endpoint);
        }

        /// <summary>
        /// Sends the request to the specified endpoint using the specified HTTP method.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the request.</param>
        /// <param name="httpMethod">The <see cref="HttpMethod"/> to use in the request.</param>
        /// <returns>The HTTP response object.</returns>
        public VerifiableResponse Invoke(string endpoint, HttpMethod httpMethod)
        {
            return this.Send(httpMethod, endpoint);
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

            this.multipartFormDataContent?.Dispose();
            this.request.Dispose();
            this.disposed = true;
        }

        /// <summary>
        /// Sends the request object to the <see cref="RequestExecutor"/>.
        /// </summary>
        /// <param name="httpMethod">The HTTP method to use in the request.</param>
        /// <param name="endpoint">The endpoint to be used in the request.</param>
        /// <returns>An object representing the HTTP response corresponding to the request.</returns>
        private VerifiableResponse Send(HttpMethod httpMethod, string endpoint)
        {
            var context = new RequestContext
            {
                HttpClient = this.httpClient,
                Request = this.request,
                CookieCollection = this.cookieCollection,
                RequestSpecification = this.requestSpecification,
                PathParams = this.pathParams,
                QueryParams = this.queryParams,
                RequestBody = this.requestBody,
                ContentTypeHeader = this.contentTypeHeader,
                ContentEncoding = this.contentEncoding,
                StripCharset = this.stripCharset,
                JsonSerializerSettings = this.jsonSerializerSettings,
                FormData = this.formData,
                MultipartFormDataContent = this.multipartFormDataContent,
                DisableSslCertificateValidation = this.disableSslCertificateValidation,
                Proxy = this.proxy,
                NetworkCredential = this.networkCredential,
                Timeout = this.timeout,
                HttpCompletionOption = this.httpCompletionOption,
                RequestLoggingLevel = this.RequestLoggingLevel,
                ResponseLoggingLevel = this.ResponseLoggingLevel,
                SensitiveRequestHeadersAndCookies = this.sensitiveRequestHeadersAndCookies,
                LogConfiguration = this.LogConfiguration,
            };

            return RequestExecutor.Send(httpMethod, endpoint, context);
        }
    }
}
