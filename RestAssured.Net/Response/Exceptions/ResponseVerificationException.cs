// <copyright file="ResponseVerificationException.cs" company="On Test Automation">
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
namespace RestAssured.Response.Exceptions
{
    using System;

    /// <summary>
    /// An exception to be thrown whenever validation of a response fails for technical reasons.
    /// </summary>
    public class ResponseVerificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseVerificationException"/> class.
        /// </summary>
        public ResponseVerificationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseVerificationException"/> class.
        /// </summary>
        /// <param name="message">The message to assign to the exception being thrown.</param>
        public ResponseVerificationException(string? message)
            : base(message)
        {
        }
    }
}
