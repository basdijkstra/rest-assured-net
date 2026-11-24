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
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.WebUtilities;
    using Newtonsoft.Json;
    using RestAssured.Logging;
    using RestAssured.Request.Builders;
    using RestAssured.Request.Exceptions;
    using RestAssured.Response;

    /// <summary>
    /// Orchestrates the execution of an HTTP request from a fully-configured <see cref="RequestContext"/>.
    /// </summary>
    internal static class RequestExecutor
    {
        /// <summary>
        /// The Content-Type used when neither the request nor the request specification sets one.
        /// </summary>
        private const string DefaultContentType = "application/json";

        /// <summary>
        /// Matches a path parameter placeholder such as [userId] and captures the name between the brackets.
        /// </summary>
        private static readonly Regex PathParamPlaceholder = new Regex(@"\[([^\[\]]+)\]", RegexOptions.Compiled);

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

            // Replace any path parameter placeholders that have been specified with their values.
            // Only placeholders whose name was supplied as a path parameter are replaced, values are inserted
            // as they are, and any other text between square brackets (e.g. filter[name]=x) is left unchanged.
            if (context.PathParams.Count > 0)
            {
                endpoint = PathParamPlaceholder.Replace(
                    endpoint,
                    match => context.PathParams.TryGetValue(match.Groups[1].Value.Trim(), out string? value) ? value : match.Value);
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

            // Values set in the request take precedence over values set in the request specification,
            // and the defaults are only used when neither of them sets a value.
            var bodySettings = new RequestBodySettings(
                context.ContentTypeHeader ?? context.RequestSpecification?.ContentType ?? DefaultContentType,
                context.ContentEncoding ?? context.RequestSpecification?.ContentEncoding ?? Encoding.UTF8,
                context.StripCharset,
                context.JsonSerializerSettings ?? context.RequestSpecification?.JsonSerializerSettings ?? new JsonSerializerSettings());

            context.Request.Content = RequestBodyFactory.Create(
                context.MultipartFormDataContent,
                context.FormData,
                context.RequestBody,
                bodySettings);

            // Proxy, SSL and NTLM settings cannot be applied to an HttpClient supplied by the user.
            if (context.HttpClient != null)
            {
                CheckSettingsForSuppliedHttpClient(context);
            }

            // SSL validation can be disabled in a request, through a RequestSpecification or in the global configuration
            bool disableSslChecks = context.DisableSslCertificateValidation || context.GlobalDisableSslCertificateValidation || (context.RequestSpecification?.DisableSslCertificateValidation ?? false);

            // Create the HTTP request processor that sends the request and set its properties
            HttpRequestProcessor httpRequestProcessor = new HttpRequestProcessor(context.HttpClient, context.Proxy ?? context.RequestSpecification?.Proxy, disableSslChecks, context.NetworkCredential);

            // Timeout set in test has precedence over timeout set in request specification.
            // If both are null, the HttpRequestProcessor uses its default timeout (100 seconds),
            // or the timeout of the HttpClient when the user supplied their own HttpClient.
            TimeSpan? timeout = context.Timeout ?? context.RequestSpecification?.Timeout;

            // HttpCompletionOption set in the test takes precedence over the value in the RequestSpecification
            // Only if it's still the default (so not set in the test), overwrite it with the value in the RequestSpecification.
            if (context.RequestSpecification != null && context.HttpCompletionOption.Equals(HttpCompletionOption.ResponseContentRead))
            {
                context.HttpCompletionOption = context.RequestSpecification.HttpCompletionOption;
            }

            if (context.RequestSpecification != null)
            {
                // Apply logging settings from the request specification,
                // but only if they haven't been set for this specific request.
                context.LogConfiguration ??= context.RequestSpecification.LogConfiguration;
            }

            var logger = new RequestResponseLogger(context.LogConfiguration ?? new LogConfiguration(), context.Logger);

            logger.LogRequest(context.Request, context.CookieCollection);

            VerifiableResponse verifiableResponse;

            try
            {
                verifiableResponse = httpRequestProcessor.Send(context.Request, context.CookieCollection, context.HttpCompletionOption, timeout).GetAwaiter().GetResult();
            }
            catch (HttpRequestProcessorException)
            {
                // Already contains the original message and exception (e.g. a refused connection or an invalid certificate).
                throw;
            }
            catch (TimeoutException te)
            {
                // The message contains the timeout that was actually exceeded.
                throw new HttpRequestProcessorException(te.Message, te);
            }
            catch (Exception ex)
            {
                throw new HttpRequestProcessorException($"Unhandled exception {ex.Message}", ex);
            }

            // Logging is done outside the try block, so that an exception thrown by the logger
            // is passed on unchanged instead of being reported as an error while sending the request.
            verifiableResponse.Logger = context.Logger;
            return logger.LogResponse(verifiableResponse);
        }

        /// <summary>
        /// Checks for proxy, SSL and NTLM settings, which cannot be applied to an <see cref="HttpClient"/> supplied by the user,
        /// because they are properties of the handler that the <see cref="HttpClient"/> was created with.
        /// Settings made in the request itself cause a <see cref="RequestCreationException"/>, because the test explicitly asks for something that cannot be done.
        /// Settings made in a <see cref="RequestSpecification"/> or in the global configuration are shared defaults, so they cause a warning in the log instead.
        /// </summary>
        /// <param name="context">The <see cref="RequestContext"/> containing all request state.</param>
        /// <exception cref="RequestCreationException">Thrown when the request itself contains settings that cannot be applied.</exception>
        private static void CheckSettingsForSuppliedHttpClient(RequestContext context)
        {
            var requestSettings = new List<string>();

            if (context.Proxy != null)
            {
                requestSettings.Add("Proxy");
            }

            if (context.DisableSslCertificateValidation)
            {
                requestSettings.Add("DisableSslCertificateValidation");
            }

            if (context.NetworkCredential != null)
            {
                requestSettings.Add("NtlmAuth");
            }

            if (requestSettings.Count > 0)
            {
                throw new RequestCreationException(
                    $"{string.Join(", ", requestSettings)} cannot be applied to a user-supplied HttpClient. Configure them on the handler of the HttpClient instead.");
            }

            var ignoredSettings = new List<string>();

            if (context.RequestSpecification?.Proxy != null)
            {
                ignoredSettings.Add("Proxy (from RequestSpecification)");
            }

            if (context.RequestSpecification?.DisableSslCertificateValidation ?? false)
            {
                ignoredSettings.Add("DisableSslCertificateValidation (from RequestSpecification)");
            }

            if (context.GlobalDisableSslCertificateValidation)
            {
                ignoredSettings.Add("DisableSslCertificateValidation (from global configuration)");
            }

            if (ignoredSettings.Count > 0)
            {
                IRestAssuredNetLogger logger = context.Logger ?? new RestAssuredNetConsoleLogger();
                logger.Log($"Warning: {string.Join(", ", ignoredSettings)} ignored, because a user-supplied HttpClient is used.");
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
                // Stryker disable once String: Only observable on Linux and macOS, where '/path' is parsed as a file URI
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
