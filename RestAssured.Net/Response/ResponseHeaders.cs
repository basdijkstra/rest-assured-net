// <copyright file="ResponseHeaders.cs" company="On Test Automation">
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
namespace RestAssured.Response
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Http;

    /// <summary>
    /// Provides access to the headers of an <see cref="HttpResponseMessage"/>.
    /// </summary>
    internal static class ResponseHeaders
    {
        /// <summary>
        /// Gets the first value of the response header with the specified name.
        /// Both the response headers and the content headers (headers that describe the response body,
        /// such as Content-Type, Content-Disposition and Last-Modified) are searched, because .NET keeps them separately.
        /// Header names are matched case-insensitively.
        /// </summary>
        /// <param name="response">The response to search the headers of.</param>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The first value of the header, if it was found.</param>
        /// <returns>True if the header was found, false otherwise.</returns>
        internal static bool TryGetFirstValue(HttpResponseMessage response, string name, [NotNullWhen(true)] out string? value)
        {
            if (response.Headers.TryGetValues(name, out IEnumerable<string>? values) ||
                response.Content.Headers.TryGetValues(name, out values))
            {
                // Stryker disable once Linq: Equivalent mutant - TryGetValues guarantees non-empty collection
                value = values.First();
                return true;
            }

            value = null;
            return false;
        }
    }
}
