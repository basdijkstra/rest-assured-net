// <copyright file="Response.cs" company="On Test Automation">
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
using RestAssuredNet.RA.Exceptions;

namespace RestAssuredNet.RA
{
    /// <summary>
    /// A class representing the response of an HTTP call.
    /// </summary>
    public class Response
    {
        private readonly int statusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code that is associated with this response.</param>
        public Response(int statusCode)
        {
            this.statusCode = statusCode;
        }

        /// <summary>
        /// Syntactic sugar (for now) that indicates the start of the 'Assert' part of a test.
        /// </summary>
        /// <returns>The current <see cref="Response"/> object.</returns>
        public Response Then()
        {
            return this;
        }

        /// <summary>
        /// A method to verify that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code.</param>
        /// <returns>The current <see cref="Response"/> object.</returns>
        /// <exception cref="AssertionException">Thrown when the actual status code does not match the expected one.</exception>
        public Response StatusCode(int expectedStatusCode)
        {
            if (!(expectedStatusCode == this.statusCode))
            {
                throw new AssertionException($"Expected status code to be {expectedStatusCode}, but was {this.statusCode}");
            }

            return this;
        }
    }
}
