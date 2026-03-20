// <copyright file="ErrorMessage.cs" company="On Test Automation">
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
namespace RestAssured.Response
{
    /// <summary>
    /// Represents an optional custom error message used in assertion failure exceptions
    /// instead of the default one. Supports Stubble template tokens <c>[expected]</c>
    /// and <c>[actual]</c> for inserting the matcher and actual value respectively.
    /// </summary>
    public readonly struct ErrorMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage"/> struct.
        /// </summary>
        /// <param name="value">The error message text.</param>
        public ErrorMessage(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the error message text, or <c>null</c> if no message was supplied.
        /// </summary>
        public string? Value { get; }

        /// <summary>
        /// Gets a value indicating whether a message was supplied.
        /// </summary>
        public bool HasValue => this.Value != null;

        /// <summary>
        /// Implicitly converts a string to an <see cref="ErrorMessage"/>.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        public static implicit operator ErrorMessage(string? value) =>
            value != null ? new ErrorMessage(value) : default;
    }
}
