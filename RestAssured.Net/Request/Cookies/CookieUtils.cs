// <copyright file="CookieUtils.cs" company="On Test Automation">
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
namespace RestAssured.Request.Cookies
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// Utility class and methods to handle and process cookies.
    /// </summary>
    internal class CookieUtils
    {
        /// <summary>
        /// Sets the domain of the <see cref="Cookie"/> to the specified host value if it isn't set already.
        /// </summary>
        /// <param name="cookie">The <see cref="Cookie"/> to be processed.</param>
        /// <param name="hostname">The domain value to be set on the cookie if it isn't set already.</param>
        /// <returns>The <see cref="Cookie"/> with the domain value set.</returns>
        internal Cookie SetDomainFor(Cookie cookie, string hostname)
        {
            // The domain for a cookie cannot be empty, so set it to the specified hostname
            // if it has not been set already
            if (string.IsNullOrEmpty(cookie.Domain))
            {
                cookie.Domain = hostname;
            }

            return cookie;
        }

        /// <summary>
        /// Returns a new <see cref="CookieContainer"/> holding only the cookies that were set by the server.
        /// Cookies that were sent with the request and not set again by the server are left out.
        /// </summary>
        /// <param name="handlerCookieContainer">The <see cref="CookieContainer"/> used by the HTTP client handler to send and receive cookies.</param>
        /// <param name="requestCookies">The cookies that were sent with the request.</param>
        /// <param name="response">The <see cref="HttpResponseMessage"/> returned by the server.</param>
        /// <returns>A <see cref="CookieContainer"/> holding the cookies set by the server.</returns>
        internal CookieContainer GetResponseCookies(CookieContainer handlerCookieContainer, CookieCollection requestCookies, HttpResponseMessage response)
        {
            CookieContainer responseCookies = new CookieContainer();

            // Cookies collected by the handler, which includes cookies set on intermediate redirect responses.
            // Cookies that are identical to a cookie sent with the request were not (necessarily) set by the server.
            foreach (Cookie cookie in handlerCookieContainer.GetAllCookies())
            {
                if (!IsSentWithRequest(cookie, requestCookies))
                {
                    responseCookies.Add(cookie);
                }
            }

            // Cookies set by the final response. This also covers cookies that the server sets with
            // the same value as a cookie sent with the request, and responses received by a custom HttpClient.
            Uri? responseUri = response.RequestMessage?.RequestUri;

            if (responseUri != null && response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? setCookieHeaders))
            {
                foreach (string setCookieHeader in setCookieHeaders)
                {
                    try
                    {
                        responseCookies.SetCookies(responseUri, setCookieHeader);
                    }
                    catch (CookieException)
                    {
                        // Ignore invalid cookies, in the same way the HTTP client handler does.
                    }
                }
            }

            return responseCookies;
        }

        private static bool IsSentWithRequest(Cookie cookie, CookieCollection requestCookies)
        {
            foreach (Cookie requestCookie in requestCookies)
            {
                if (requestCookie.Name.Equals(cookie.Name, StringComparison.Ordinal) &&
                    requestCookie.Value.Equals(cookie.Value, StringComparison.Ordinal) &&
                    NormalizeDomain(requestCookie.Domain).Equals(NormalizeDomain(cookie.Domain), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string NormalizeDomain(string domain)
        {
            return domain.TrimStart('.');
        }
    }
}
