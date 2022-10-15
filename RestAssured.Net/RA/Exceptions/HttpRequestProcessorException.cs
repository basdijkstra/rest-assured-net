// <copyright file="HttpRequestProcessorException.cs" company="On Test Automation">
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
using System;

namespace RestAssuredNet.RA.Exceptions
{
    /// <summary>
    /// An exception to be thrown whenever sending an HTTP request fails.
    /// </summary>
    public class HttpRequestProcessorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestProcessorException"/> class.
        /// </summary>
        public HttpRequestProcessorException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestProcessorException"/> class.
        /// </summary>
        /// /// <param name="message">The message to assign to the exception being thrown.</param>
        public HttpRequestProcessorException(string message)
            : base(message)
        {
        }
    }
}
