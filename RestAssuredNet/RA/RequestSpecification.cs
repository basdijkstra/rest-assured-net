// <copyright file="RequestSpecification.cs" company="On Test Automation">
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
using RestAssuredNet.RA.Internal;

namespace RestAssuredNet.RA
{
    /// <summary>
    /// The request to be sent.
    /// </summary>
    public class RequestSpecification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSpecification"/> class.
        /// </summary>
        public RequestSpecification()
        {
        }

        /// <summary>
        /// Syntactic sugar (for now) to help indicate the start of the 'Act' part of a test.
        /// </summary>
        /// <returns>The current <see cref="RequestSpecification"/>.</returns>
        public RequestSpecification When()
        {
            return this;
        }

        /// <summary>
        /// Performs an HTTP GET.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP GET request.</param>
        /// <returns>The HTTP response object.</returns>
        public Response Get(string endpoint)
        {
            Task<Response> task = HttpRequestProcessor.Get(endpoint);
            return task.Result;
        }

        /// <summary>
        /// Performs an HTTP POST.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP POST request.</param>
        /// <returns>The HTTP response object.</returns>
        public Response Post(string endpoint)
        {
            Task<Response> task = HttpRequestProcessor.Post(endpoint);
            return task.Result;
        }

        /// <summary>
        /// Performs an HTTP PATCH.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP PATCH request.</param>
        /// <returns>The HTTP response object.</returns>
        public Response Patch(string endpoint)
        {
            Task<Response> task = HttpRequestProcessor.Patch(endpoint);
            return task.Result;
        }

        /// <summary>
        /// Performs an HTTP DELETE.
        /// </summary>
        /// <param name="endpoint">The endpoint to invoke in the HTTP DELETE request.</param>
        /// <returns>The HTTP response object.</returns>
        public Response Delete(string endpoint)
        {
            Task<Response> task = HttpRequestProcessor.Delete(endpoint);
            return task.Result;
        }
    }
}
