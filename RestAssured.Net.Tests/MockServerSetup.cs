// <copyright file="MockServerSetup.cs" company="On Test Automation">
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
    /// Starts the WireMock servers once for the entire test run and stops them afterwards.
    /// Starting and stopping a server for every test can fail intermittently when the port
    /// of the previous server has not been released yet.
    /// </summary>
    [SetUpFixture]
    public class MockServerSetup
    {
        /// <summary>
        /// The port used by the HTTP WireMock server.
        /// </summary>
        internal const int HttpPort = 9876;

        /// <summary>
        /// The port used by the HTTPS WireMock server.
        /// </summary>
        internal const int HttpsPort = 8443;

        /// <summary>
        /// Gets the HTTP WireMock server shared by all tests.
        /// </summary>
        internal static WireMockServer HttpServer { get; private set; }

        /// <summary>
        /// Gets the HTTPS WireMock server shared by all tests.
        /// </summary>
        internal static WireMockServer HttpsServer { get; private set; }

        /// <summary>
        /// Starts the WireMock servers before any test is run.
        /// </summary>
        [OneTimeSetUp]
        public void StartServers()
        {
            HttpServer = WireMockServer.Start(HttpPort);
            HttpsServer = WireMockServer.Start(port: HttpsPort, useSSL: true);
        }

        /// <summary>
        /// Stops the WireMock servers after all tests have been run.
        /// </summary>
        [OneTimeTearDown]
        public void StopServers()
        {
            HttpServer?.Stop();
            HttpsServer?.Stop();
        }
    }
}
