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
        /// Returns a copy of the <see cref="Cookie"/>, with the domain set to the specified host value if it isn't set already.
        /// A copy is returned, so that the <see cref="Cookie"/> supplied by the user is never modified.
        /// </summary>
        /// <param name="cookie">The <see cref="Cookie"/> to be copied.</param>
        /// <param name="hostname">The domain value to be set on the copy if the cookie has no domain.</param>
        /// <returns>A copy of the <see cref="Cookie"/> with the domain value set.</returns>
        internal Cookie CopyWithDomain(Cookie cookie, string hostname)
        {
            // System.Net.Cookie has no public Clone() method, so all properties that affect sending the cookie are copied.
            // The domain for a cookie cannot be empty, so it is set to the specified hostname if it has not been set already.
            Cookie copy = new Cookie(cookie.Name, cookie.Value, cookie.Path, string.IsNullOrEmpty(cookie.Domain) ? hostname : cookie.Domain)
            {
                Secure = cookie.Secure,
                HttpOnly = cookie.HttpOnly,
                Expires = cookie.Expires,
                Version = cookie.Version,
                Comment = cookie.Comment,
                CommentUri = cookie.CommentUri,
                Discard = cookie.Discard,
            };

            if (!string.IsNullOrEmpty(cookie.Port))
            {
                copy.Port = cookie.Port;
            }

            return copy;
        }

        /// <summary>
        /// Returns copies of the cookies in the <see cref="CookieCollection"/>, each with the domain set to the specified host value if it isn't set already.
        /// </summary>
        /// <param name="cookies">The <see cref="CookieCollection"/> to be copied.</param>
        /// <param name="hostname">The domain value to be set on the copies of cookies that have no domain.</param>
        /// <returns>A new <see cref="CookieCollection"/> holding the copies.</returns>
        internal CookieCollection CopyWithDomain(CookieCollection cookies, string hostname)
        {
            CookieCollection copies = new CookieCollection();

            foreach (Cookie cookie in cookies)
            {
                copies.Add(this.CopyWithDomain(cookie, hostname));
            }

            return copies;
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
