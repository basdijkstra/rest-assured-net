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
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.AspNetCore.WebUtilities;
    using Newtonsoft.Json;
    using RestAssured.Configuration;
    using RestAssured.Request.Builders;
    using RestAssured.Request.Exceptions;
    using RestAssured.Request.Logging;
    using RestAssured.Response;
    using RestAssured.Response.Logging;
    using Stubble.Core;
    using Stubble.Core.Builders;
    using Stubble.Core.Classes;

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
        private bool disableSslCertificateValidation = false;
        private bool disposed = false;

        /// <summary>
        /// The request logging level for this request.
        /// </summary>
        internal RequestLogLevel RequestLoggingLevel { get; set; }

        /// <summary>
        /// The response logging level for this request.
        /// </summary>
        internal ResponseLogLevel ResponseLoggingLevel { get; set; }

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
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest Header(string key, object value)
        {
            this.request.Headers.Add(key, value.ToString());
            return this;
        }

        /// <summary>
        /// Adds a request header and the associated values to the request object to be sent.
        /// </summary>
        /// <param name="key">The header key that is to be added to the request.</param>
        /// <param name="values">The associated header values that are to be added to the request.</param>
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest Header(string key, IEnumerable<string> values)
        {
            this.request.Headers.Add(key, values);
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
                contentType ??= this.GetContentTypeForFile(fileName);

                StreamContent fileContents = new StreamContent(fileName.OpenRead());
                fileContents.Headers.ContentType = contentType;

                this.multipartFormDataContent.Add(fileContents, controlName, fileName.Name);
            }
            catch (IOException ioe)
            {
                throw new RequestCreationException(ioe.Message);
            }

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
        /// Logs request details to the standard output.
        /// </summary>
        /// <param name="requestLogLevel">The desired request log level.</param>
        /// <param name="sensitiveHeaderOrCookieNames">The names of the request headers or cookies to be masked when logging.</param>
        /// <returns>The current <see cref="ExecutableRequest"/> object.</returns>
        public ExecutableRequest Log(RequestLogLevel requestLogLevel, List<string>? sensitiveHeaderOrCookieNames = null)
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
        /// <returns>The current <see cref="ExecutableRequest"/>.</returns>
        public ExecutableRequest Body(object body)
        {
            this.requestBody = body;
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
        /// Returns the associated <see cref="HttpRequestMessage"/> (for logging purposes).
        /// </summary>
        /// <returns>The <see cref="HttpRequestMessage"/> associated with this request.</returns>
        internal HttpRequestMessage GetRequest()
        {
            return this.request;
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
        /// Sends the request object to the <see cref="HttpRequestProcessor"/>.
        /// </summary>
        /// <param name="httpMethod">The HTTP method to use in the request.</param>
        /// <param name="endpoint">The endpoint to be used in the request.</param>
        /// <returns>An object representing the HTTP response corresponding to the request.</returns>
        private VerifiableResponse Send(HttpMethod httpMethod, string endpoint)
        {
            // Set the HTTP method for the request
            this.request.Method = httpMethod;

            // Replace any path parameter placeholders that have been specified with their values
            if (this.pathParams.Count > 0)
            {
                StubbleVisitorRenderer renderer = new StubbleBuilder()
                    .Configure(builder => builder.SetDefaultTags(new Tags("[", "]")))
                    .Build();
                endpoint = renderer.Render(endpoint, this.pathParams);
            }

            // Build the Uri for the request
            this.request.RequestUri = this.BuildUri(this.requestSpecification!, endpoint);

            // Add any query parameters that have been specified and create the endpoint
            foreach (KeyValuePair<string, string> param in this.queryParams)
            {
                this.request.RequestUri = new Uri(QueryHelpers.AddQueryString(this.request.RequestUri.ToString(), param.Key, param.Value));
            }

            // Apply other settings provided in the request specification to the request
            this.request = RequestSpecificationProcessor.Apply(this.requestSpecification!, this.request);

            this.request.Content = this.CreateRequestBody();

            // SSL validation can be disabled either in a request or through a RequestSpecification
            bool disableSslChecks = this.disableSslCertificateValidation || (this.requestSpecification?.DisableSslCertificateValidation ?? false);

            // Create the HTTP request processor that sends the request and set its properties
            HttpRequestProcessor httpRequestProcessor = new HttpRequestProcessor(this.httpClient, this.proxy ?? this.requestSpecification?.Proxy, disableSslChecks);

            // Timeout set in test has precedence over timeout set in request specification
            // If both are null, use default timeout for HttpClient (= 100.000 milliseconds).
            if (this.timeout != null)
            {
                httpRequestProcessor.SetTimeout((TimeSpan)this.timeout);
            }
            else if (this.requestSpecification != null)
            {
                if (this.requestSpecification.Timeout != null)
                {
                    httpRequestProcessor.SetTimeout((TimeSpan)this.requestSpecification.Timeout);
                }
            }

            // Add header and cookie values to be masked specified in RequestSpecification to the list
            if (this.requestSpecification != null)
            {
                this.sensitiveRequestHeadersAndCookies.AddRange(this.requestSpecification.SensitiveRequestHeadersAndCookies);
            }

            RequestLogger.LogToConsole(this.request, this.RequestLoggingLevel, this.cookieCollection, this.sensitiveRequestHeadersAndCookies);

            try
            {
                Task<VerifiableResponse> task = httpRequestProcessor.Send(this.request, this.cookieCollection);
                VerifiableResponse verifiableResponse = task.Result;
                verifiableResponse.Log(this.ResponseLoggingLevel);
                return verifiableResponse;
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException?.GetType() == typeof(TaskCanceledException))
                {
                    throw new HttpRequestProcessorException($"Request timeout of {this.timeout ?? this.requestSpecification?.Timeout ?? TimeSpan.FromSeconds(100)} exceeded.");
                }

                throw new HttpRequestProcessorException($"Unhandled exception {ae.Message}");
            }
        }

        /// <summary>
        /// Builds the URI to be used in the request from the current endpoint and the <see cref="RequestSpecification"/>.
        /// </summary>
        /// <param name="requestSpec">The <see cref="RequestSpecification"/> to use when constructing the endpoint.</param>
        /// <param name="endpoint">The endpoint as supplied in the test.</param>
        /// <returns>The modified endpoint to use in the request.</returns>
        private Uri BuildUri(RequestSpecification requestSpec, string endpoint)
        {
            try
            {
                Uri uri = new Uri(endpoint);

                // '/path' does not throw an UriFormatException on Linux and MacOS,
                // but creates a Uri 'file://path', which we do not want to use here.
                // See also https://github.com/dotnet/runtime/issues/27813
                if (uri.Scheme != "file")
                {
                    // All OSes, absolute path
                    return uri;
                }

                // MacOS, Unix, relative path
                return RequestSpecificationProcessor.BuildUriFromRequestSpec(requestSpec, endpoint);
            }
            catch (UriFormatException)
            {
                // Windows, relative path
                return RequestSpecificationProcessor.BuildUriFromRequestSpec(requestSpec, endpoint);
            }
        }

        /// <summary>
        /// Creates the body payload for the request.
        /// </summary>
        /// <returns>The request body as an object of type <see cref="HttpContent"/>.</returns>
        private HttpContent CreateRequestBody()
        {
            if (this.multipartFormDataContent != null)
            {
                return this.multipartFormDataContent;
            }

            if (this.formData != null)
            {
                return new FormUrlEncodedContent(this.formData);
            }

            string requestBodyAsString = this.Serialize(this.requestBody, this.requestSpecification?.ContentType ?? this.contentTypeHeader);

            return new StringContent(requestBodyAsString, this.requestSpecification?.ContentEncoding ?? this.contentEncoding, this.requestSpecification?.ContentType ?? this.contentTypeHeader);
        }

        /// <summary>
        /// Serializes the request body set for the request object to JSON, if necessary.
        /// </summary>
        /// <param name="body">The request body object.</param>
        /// <param name="contentType">The request Content-Type header value.</param>
        /// <returns>Either the body itself (if the body is a string), or a serialized version of the body.</returns>
        private string Serialize(object body, string contentType)
        {
            if (body is string)
            {
                return (string)body;
            }

            if (contentType.Contains("json"))
            {
                return JsonConvert.SerializeObject(body, this.requestSpecification?.JsonSerializerSettings ?? this.jsonSerializerSettings);
            }

            if (contentType.Contains("xml"))
            {
                using (StringWriter sw = new StringWriter())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(body.GetType());
                    xmlSerializer.Serialize(sw, body);
                    return sw.ToString();
                }
            }

            throw new RequestCreationException(
                $"Could not determine how to serialize request based on specified content type '{contentType}'");
        }

        private MediaTypeHeaderValue GetContentTypeForFile(FileInfo fileName)
        {
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();

            string contentType;

            if (!provider.TryGetContentType(fileName.FullName, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return MediaTypeHeaderValue.Parse(contentType);
        }

        private string GetContentTypeForFile(string fileName)
        {
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();

            string contentType;

            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
