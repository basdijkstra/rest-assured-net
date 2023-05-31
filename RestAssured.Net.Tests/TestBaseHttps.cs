// <copyright file="TestBaseHttps.cs" company="On Test Automation">
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
    using NUnit.Framework;
    using WireMock.Server;

    /// <summary>
    /// Base class containing common test logic.
    /// </summary>
    public class TestBaseHttps
    {
        /// <summary>
        /// The WireMock server instance to which response definitions will be added.
        /// </summary>
        protected WireMockServer Server { get; private set; }

        /// <summary>
        /// Starts the WireMock server before every test.
        /// </summary>
        [SetUp]
        public void StartServer()
        {
            this.Server = WireMockServer.Start(port: 8443, ssl: true);
        }

        /// <summary>
        /// Stops the WireMock server after every test.
        /// </summary>
        [TearDown]
        public void StopServer()
        {
            this.Server?.Stop();
        }
    }
}
