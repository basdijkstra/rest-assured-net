// <copyright file="ResponseLogLevel.cs" company="On Test Automation">
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

namespace RestAssured.Response.Logging
{
    /// <summary>
    /// Contains the different logging levels for request logging.
    /// </summary>
    public enum ResponseLogLevel
    {
        /// <summary>
        /// Nothing will be logged to the console.
        /// </summary>
        None = 0,

        /// <summary>
        /// Response headers will be logged to the console.
        /// </summary>
        Headers = 1,

        /// <summary>
        /// Response body will be logged to the console.
        /// </summary>
        Body = 2,

        /// <summary>
        /// Response time will be logged to the console.
        /// </summary>
        ResponseTime = 3,

        /// <summary>
        /// All response details will be logged to the console.
        /// </summary>
        All = 4,

        /// <summary>
        /// The entire response will be logged to the console
        /// if the response status code is 4xx or 5xx.
        /// </summary>
        OnError = 5,

        /// <summary>
        /// The entire response will be logged to the console
        /// if any of the response verifications fails.
        /// </summary>
        OnVerificationFailure = 6,
    }
}
