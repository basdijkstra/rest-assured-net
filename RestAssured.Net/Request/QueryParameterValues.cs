// <copyright file="QueryParameterValues.cs" company="On Test Automation">
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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Converts query parameter values to the key/value pairs that are added to the request URI.
    /// </summary>
    internal static class QueryParameterValues
    {
        /// <summary>
        /// Converts a query parameter value to one or more key/value pairs.
        /// A collection (or array) value results in one key/value pair per element, in order.
        /// Any other value, including a string (which is also a collection of characters), results in a single key/value pair.
        /// </summary>
        /// <param name="key">The query parameter name.</param>
        /// <param name="value">The query parameter value.</param>
        /// <returns>The key/value pairs to add to the request URI.</returns>
        /// <remarks>
        /// The values are read immediately, so that changing the collection afterwards does not change the request,
        /// and a collection that can only be enumerated once still works when a request specification is reused.
        /// </remarks>
        internal static List<KeyValuePair<string, string>> Expand(string key, object value)
        {
            var pairs = new List<KeyValuePair<string, string>>();

            if (value is IEnumerable values && value is not string)
            {
                foreach (object? element in values)
                {
                    pairs.Add(new KeyValuePair<string, string>(key, element?.ToString() ?? string.Empty));
                }
            }
            else
            {
                pairs.Add(new KeyValuePair<string, string>(key, value.ToString() ?? string.Empty));
            }

            return pairs;
        }
    }
}
