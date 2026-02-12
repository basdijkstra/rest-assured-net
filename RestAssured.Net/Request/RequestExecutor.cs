// <copyright file="RequestExecutor.cs" company="On Test Automation">
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
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.WebUtilities;
    using RestAssured.Logging;
    using RestAssured.Request.Builders;
    using RestAssured.Request.Exceptions;
    using RestAssured.Response;
    using Stubble.Core;
    using Stubble.Core.Builders;
    using Stubble.Core.Classes;

    /// <summary>
    /// Orchestrates the execution of an HTTP request from a fully-configured <see cref="RequestContext"/>.
    /// </summary>
    internal static class RequestExecutor
    {
        /// <summary>
        /// Sends the request described by the given <see cref="RequestContext"/>.
        /// </summary>
        /// <param name="httpMethod">The HTTP method to use in the request.</param>
        /// <param name="endpoint">The endpoint to be used in the request.</param>
        /// <param name="context">The <see cref="RequestContext"/> containing all request state.</param>
        /// <returns>An object representing the HTTP response corresponding to the request.</returns>
        internal static VerifiableResponse Send(HttpMethod httpMethod, string endpoint, RequestContext context)
        {
            // Set the HTTP method for the request
            context.Request.Method = httpMethod;

            // Replace any path parameter placeholders that have been specified with their values
            if (context.PathParams.Count > 0)
            {
                StubbleVisitorRenderer renderer = new StubbleBuilder()
                    .Configure(builder => builder.SetDefaultTags(new Tags("[", "]")))
                    .Build();
                endpoint = renderer.Render(endpoint, context.PathParams);
            }

            // Build the Uri for the request
            context.Request.RequestUri = BuildUri(context.RequestSpecification!, endpoint);

            // Add any query parameters that have been specified and create the endpoint
            if (context.RequestSpecification != null)
            {
                foreach (KeyValuePair<string, string> param in context.RequestSpecification.QueryParams)
                {
                    context.Request.RequestUri = new Uri(QueryHelpers.AddQueryString(context.Request.RequestUri.ToString(), param.Key, param.Value));
                }
            }

            foreach (KeyValuePair<string, string> param in context.QueryParams)
            {
                context.Request.RequestUri = new Uri(QueryHelpers.AddQueryString(context.Request.RequestUri.ToString(), param.Key, param.Value));
            }

            // Apply other settings provided in the request specification to the request
            context.Request = RequestSpecificationProcessor.Apply(context.RequestSpecification!, context.Request);

            var bodySettings = new RequestBodySettings(
                context.RequestSpecification?.ContentType ?? context.ContentTypeHeader,
                context.RequestSpecification?.ContentEncoding ?? context.ContentEncoding,
                context.StripCharset,
                context.RequestSpecification?.JsonSerializerSettings ?? context.JsonSerializerSettings);

            context.Request.Content = RequestBodyFactory.Create(
                context.MultipartFormDataContent,
                context.FormData,
                context.RequestBody,
                bodySettings);

            // SSL validation can be disabled either in a request or through a RequestSpecification
            bool disableSslChecks = context.DisableSslCertificateValidation || (context.RequestSpecification?.DisableSslCertificateValidation ?? false);

            // Create the HTTP request processor that sends the request and set its properties
            HttpRequestProcessor httpRequestProcessor = new HttpRequestProcessor(context.HttpClient, context.Proxy ?? context.RequestSpecification?.Proxy, disableSslChecks, context.NetworkCredential);

            // Timeout set in test has precedence over timeout set in request specification
            // If both are null, use default timeout for HttpClient (= 100.000 milliseconds).
            if (context.Timeout != null)
            {
                httpRequestProcessor.SetTimeout((TimeSpan)context.Timeout);
            }
            else if (context.RequestSpecification != null)
            {
                if (context.RequestSpecification.Timeout != null)
                {
                    httpRequestProcessor.SetTimeout((TimeSpan)context.RequestSpecification.Timeout);
                }
            }

            // HttpCompletionOption set in the test takes precedence over the value in the RequestSpecification
            // Only if it's still the default (so not set in the test), overwrite it with the value in the RequestSpecification.
            if (context.RequestSpecification != null && context.HttpCompletionOption.Equals(HttpCompletionOption.ResponseContentRead))
            {
                context.HttpCompletionOption = context.RequestSpecification.HttpCompletionOption;
            }

            var legacyLogConfiguration = new LogConfiguration
            {
                RequestLogLevel = (RequestLogLevel)context.RequestLoggingLevel,
                ResponseLogLevel = (ResponseLogLevel)context.ResponseLoggingLevel,
                SensitiveRequestHeadersAndCookies = context.SensitiveRequestHeadersAndCookies,
                SensitiveResponseHeadersAndCookies = new List<string>(),
            };

            if (context.RequestSpecification != null)
            {
                // Apply logging settings from the request specification,
                // but only if they haven't been set for this specific request.
                context.LogConfiguration ??= context.RequestSpecification.LogConfiguration;
            }

            // Add header and cookie values to be masked specified in RequestSpecification to the list
            if (context.RequestSpecification != null)
            {
                context.SensitiveRequestHeadersAndCookies.AddRange(context.RequestSpecification.SensitiveRequestHeadersAndCookies);
            }

            var logger = new RequestResponseLogger(context.LogConfiguration ?? legacyLogConfiguration);

            logger.LogRequest(context.Request, context.CookieCollection);

            try
            {
                VerifiableResponse verifiableResponse = httpRequestProcessor.Send(context.Request, context.CookieCollection, context.HttpCompletionOption).GetAwaiter().GetResult();
                verifiableResponse = logger.LogResponse(verifiableResponse);
                return verifiableResponse;
            }
            catch (TaskCanceledException)
            {
                throw new HttpRequestProcessorException($"Request timeout of {context.Timeout ?? context.RequestSpecification?.Timeout ?? TimeSpan.FromSeconds(100)} exceeded.");
            }
            catch (Exception ex)
            {
                throw new HttpRequestProcessorException($"Unhandled exception {ex.Message}");
            }
        }

        /// <summary>
        /// Builds the URI to be used in the request from the current endpoint and the <see cref="RequestSpecification"/>.
        /// </summary>
        /// <param name="requestSpec">The <see cref="RequestSpecification"/> to use when constructing the endpoint.</param>
        /// <param name="endpoint">The endpoint as supplied in the test.</param>
        /// <returns>The modified endpoint to use in the request.</returns>
        private static Uri BuildUri(RequestSpecification requestSpec, string endpoint)
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
    }
}
