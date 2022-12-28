// <copyright file="RequestLogLevel.cs" company="On Test Automation">
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

namespace RestAssured.Request.Logging
{
    /// <summary>
    /// Contains the different logging levels for request logging.
    /// </summary>
    public enum RequestLogLevel
    {
        /// <summary>
        /// Nothing will be logged to the console.
        /// </summary>
        None = 0,

        /// <summary>
        /// The HTTP method and the endpoint will be logged to the console.
        /// </summary>
        Endpoint = 1,

        /// <summary>
        /// Request headers will be logged to the console.
        /// </summary>
        Headers = 2,

        /// <summary>
        /// Request body will be logged to the console.
        /// </summary>
        Body = 3,

        /// <summary>
        /// All request details will be logged to the console.
        /// </summary>
        All = 4,
    }
}
