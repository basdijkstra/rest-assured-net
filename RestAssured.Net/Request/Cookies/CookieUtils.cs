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
    using System.Net;

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
    }
}
