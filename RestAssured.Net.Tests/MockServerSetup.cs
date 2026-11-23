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
    using System.Net;
    using System.Net.Sockets;
    using NUnit.Framework;
    using WireMock.Server;

    /// <summary>
    /// Starts the WireMock servers once for the entire test run and stops them afterwards.
    /// Starting and stopping a server for every test can fail intermittently when the port
    /// of the previous server has not been released yet.
    /// The servers use free ports that are chosen at runtime, so that several test runs
    /// (for example, parallel Stryker test runners) can run side by side on the same machine.
    /// </summary>
    [SetUpFixture]
    public class MockServerSetup
    {
        /// <summary>
        /// The port used by the HTTP WireMock server.
        /// The port is chosen when this class is first used, which can be before the servers are started,
        /// because test fixtures and their static fields are created when the tests are discovered.
        /// </summary>
        internal static readonly int HttpPort;

        /// <summary>
        /// The port used by the HTTPS WireMock server.
        /// </summary>
        internal static readonly int HttpsPort;

        /// <summary>
        /// Initializes static members of the <see cref="MockServerSetup"/> class.
        /// </summary>
        static MockServerSetup()
        {
            int[] ports = GetFreePorts(2);
            HttpPort = ports[0];
            HttpsPort = ports[1];
        }

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

        /// <summary>
        /// Asks the operating system for TCP ports that are currently free.
        /// All listeners are kept open until every port has been chosen, so the ports are guaranteed to be different.
        /// </summary>
        /// <param name="count">The number of ports to return.</param>
        /// <returns>The free TCP ports.</returns>
        private static int[] GetFreePorts(int count)
        {
            TcpListener[] listeners = new TcpListener[count];
            int[] ports = new int[count];

            try
            {
                for (int i = 0; i < count; i++)
                {
                    listeners[i] = new TcpListener(IPAddress.Loopback, 0);
                    listeners[i].Start();
                    ports[i] = ((IPEndPoint)listeners[i].LocalEndpoint).Port;
                }

                return ports;
            }
            finally
            {
                foreach (TcpListener listener in listeners)
                {
                    listener?.Stop();
                }
            }
        }
    }
}
