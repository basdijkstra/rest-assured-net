// <copyright file="RestAssuredNetConsoleLoggerTests.cs" company="On Test Automation">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;
    using RestAssured.Logging;

    /// <summary>
    /// Tests for the <see cref="RestAssuredNetConsoleLogger"/> class.
    /// </summary>
    [TestFixture]
    public class RestAssuredNetConsoleLoggerTests
    {
        /// <summary>
        /// Verifies that a message is written to the sink supplied to the logger.
        /// </summary>
        [Test]
        public void MessageIsWrittenToSuppliedSink()
        {
            var lines = new List<string>();
            var logger = new RestAssuredNetConsoleLogger(lines.Add);

            logger.Log("a log message");

            Assert.That(lines, Is.EqualTo(new[] { "a log message" }));
        }

        /// <summary>
        /// Verifies that a message is written to the standard output when no sink is supplied.
        /// </summary>
        [Test]
        public void MessageIsWrittenToStandardOutputByDefault()
        {
            TextWriter originalOut = Console.Out;
            using StringWriter capturedOut = new StringWriter();

            try
            {
                Console.SetOut(capturedOut);

                new RestAssuredNetConsoleLogger().Log("a log message");
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            Assert.That(capturedOut.ToString(), Is.EqualTo("a log message" + Environment.NewLine));
        }
    }
}
