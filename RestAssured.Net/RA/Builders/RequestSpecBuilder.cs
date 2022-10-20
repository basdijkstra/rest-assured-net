// <copyright file="RequestSpecBuilder.cs" company="On Test Automation">
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
namespace RestAssured.Net.RA.Builders
{
    /// <summary>
    /// A builder class to construct a new instance of the <see cref="RequestSpecification"/> class.
    /// </summary>
    public class RequestSpecBuilder
    {
        private readonly RequestSpecification requestSpecification = new RequestSpecification();

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSpecBuilder"/> class.
        /// </summary>
        public RequestSpecBuilder()
        {
        }

        /// <summary>
        /// Sets the base URI on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="baseUri">The base URI to use in the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithBaseUri(string baseUri)
        {
            this.requestSpecification.BaseUri = baseUri;
            return this;
        }

        /// <summary>
        /// Sets the port on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="port">The port to use in the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithPort(int port)
        {
            this.requestSpecification.Port = port;
            return this;
        }

        /// <summary>
        /// Returns the <see cref="RequestSpecification"/> that was built.
        /// </summary>
        /// <returns>The <see cref="RequestSpecification"/> object built in this builder class.</returns>
        public RequestSpecification Build()
        {
            return this.requestSpecification;
        }
    }
}
