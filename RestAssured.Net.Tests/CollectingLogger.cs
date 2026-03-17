// <copyright file="CollectingLogger.cs" company="On Test Automation">
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
namespace RestAssured.Tests
{
    using System.Collections.Generic;
    using RestAssured.Logging;

    /// <summary>
    /// A test implementation of <see cref="IRestAssuredNetLogger"/> that collects all log lines for assertion.
    /// </summary>
    public class CollectingLogger : IRestAssuredNetLogger
    {
        /// <summary>
        /// All messages received by this logger.
        /// </summary>
        public List<string> Messages { get; } = new List<string>();

        /// <inheritdoc/>
        public void Log(string message)
        {
            this.Messages.Add(message);
        }
    }
}
