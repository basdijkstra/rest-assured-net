// <copyright file="LogConfiguration.cs" company="On Test Automation">
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

namespace RestAssured.Logging
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the configuration to be used when logging request or response details.
    /// </summary>
    public class LogConfiguration
    {
        /// <summary>
        /// The <see cref="ILogger"/> instance to use when logging request and response details.
        /// </summary>
        public ILogger Logger { get; set; } = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("RestAssured.Net");

        /// <summary>
        /// The request logging level.
        /// </summary>
        public RequestLogLevel RequestLogLevel { get; set; } = RequestLogLevel.None;

        /// <summary>
        /// The response logging level.
        /// </summary>
        public ResponseLogLevel ResponseLogLevel { get; set; } = ResponseLogLevel.None;

        /// <summary>
        /// A list of sensitive request header and cookie names that should be redacted when logging request details.
        /// </summary>
        public List<string> SensitiveRequestHeadersAndCookies { get; set; } = new List<string>();

        /// <summary>
        /// A list of sensitive response header and cookie names that should be redacted when logging response details.
        /// </summary>
        public List<string> SensitiveResponseHeadersAndCookies { get; set; } = new List<string>();
    }
}
