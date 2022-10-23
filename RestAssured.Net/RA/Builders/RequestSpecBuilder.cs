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
        private readonly RequestSpecification requestSpecification;

        private readonly string scheme = "http";
        private readonly string host = "localhost";
        private readonly int port = 80;
        private readonly string basePath = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSpecBuilder"/> class.
        /// </summary>
        public RequestSpecBuilder()
        {
            this.requestSpecification = new RequestSpecification(this.scheme, this.host, this.port, this.basePath);
        }

        /// <summary>
        /// Sets the scheme (http, https, ...) on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="scheme">The scheme to use in the request.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithScheme(string scheme)
        {
            this.requestSpecification.Scheme = scheme;
            return this;
        }

        /// <summary>
        /// Sets the host name on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="host">The host name to use in the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithHostName(string host)
        {
            this.requestSpecification.HostName = host;
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
        /// Sets the base path on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="basePath">The base path to use in the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithBasePath(string basePath)
        {
            this.requestSpecification.BasePath = basePath;
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
