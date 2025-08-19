// <copyright file="RestAssuredConfiguration.cs" company="On Test Automation">
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
namespace RestAssured.Configuration
{
    using System;
    using System.Net.Http;
    using RestAssured.Logging;

    /// <summary>
    /// Contains static method for global RestAssured.Net configuration.
    /// </summary>
    public class RestAssuredConfiguration
    {
        /// <summary>
        /// Setting to disable SSL certificate validation for requests.
        /// </summary>
        public bool DisableSslCertificateValidation { get; set; } = false;

        /// <summary>
        /// Configuration for be used when logging request and response details.
        /// </summary>
        public LogConfiguration? LogConfiguration { get; set; } = new LogConfiguration();

        /// <summary>
        /// Setting to configure the <see cref="HttpCompletionOption"/> for all tests.
        /// </summary>
        public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseContentRead;
    }
}
