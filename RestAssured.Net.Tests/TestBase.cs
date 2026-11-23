// <copyright file="TestBase.cs" company="On Test Automation">
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
    using System.Linq;
    using NUnit.Framework;
    using RestAssured.Tests.Models;
    using WireMock;
    using WireMock.Server;

    /// <summary>
    /// Base class containing common test logic.
    /// </summary>
    public class TestBase
    {
        /// <summary>
        /// The WireMock server instance to which response definitions will be added.
        /// </summary>
        protected WireMockServer Server { get; private set; }

        /// <summary>
        /// Gets the last request received by the mock server, to verify what was actually sent.
        /// </summary>
        protected IRequestMessage LastRequest => this.Server.LogEntries.Last().RequestMessage;

        /// <summary>
        /// The port used by the WireMock mock server.
        /// </summary>
        protected static readonly int MOCK_SERVER_PORT = MockServerSetup.HttpPort;

        /// <summary>
        /// The base URL used by the WireMock mock server.
        /// </summary>
        protected static readonly string MOCK_SERVER_BASE_URL = $"http://localhost:{MOCK_SERVER_PORT}";

        /// <summary>
        /// Removes all stubs and request logs from the shared WireMock server before every test.
        /// The server itself is started once for the entire test run, see <see cref="MockServerSetup"/>.
        /// </summary>
        [SetUp]
        protected void ResetServer()
        {
            this.Server = MockServerSetup.HttpServer;
            this.Server.Reset();
        }

        /// <summary>
        /// Returns the values of a header in the last request received by the mock server.
        /// </summary>
        /// <param name="name">The name of the header (case-insensitive).</param>
        /// <returns>The values of the header, or an empty list if the header was not sent.</returns>
        protected List<string> GetLastRequestHeaderValues(string name)
        {
            return this.LastRequest.Headers
                .Where(header => string.Equals(header.Key, name, StringComparison.OrdinalIgnoreCase))
                .SelectMany(header => header.Value)
                .ToList();
        }

        /// <summary>
        /// Returns the individual name=value pairs of all cookies in the Cookie header(s) of the last request received by the mock server.
        /// </summary>
        /// <returns>The cookies as name=value pairs.</returns>
        protected List<string> GetLastRequestCookies()
        {
            return this.GetLastRequestHeaderValues("Cookie")
                .SelectMany(value => value.Split(';'))
                .Select(pair => pair.Trim())
                .Where(pair => pair.Length > 0)
                .ToList();
        }

        /// <summary>
        /// Returns the exact query string (including the leading '?') of the last request received by the mock server.
        /// </summary>
        /// <returns>The query string of the last request.</returns>
        protected string GetLastRequestQueryString()
        {
            return new Uri(this.LastRequest.Url).Query;
        }

        /// <summary>
        /// Returns an object of type <see cref="Location"/> to use in various tests.
        /// </summary>
        /// <returns>A <see cref="Location"/> object with test values.</returns>
        protected Location GetLocation()
        {
            Place firstPlace = new Place
            {
                Name = "Sun City",
                Inhabitants = 100000,
                IsCapital = true,
            };

            Place secondPlace = new Place
            {
                Name = "Pleasure Meadow",
                Inhabitants = 50000,
                IsCapital = false,
            };

            Location location = new Location
            {
                Country = "United States",
                State = "California",
                ZipCode = 90210,
                Places = new List<Place>() { firstPlace, secondPlace },
            };

            return location;
        }

        /// <summary>
        /// Returns an XML string representing a <see cref="Location"/>.
        /// </summary>
        /// <returns>An XML string representing a <see cref="Location"/>.</returns>
        protected string GetLocationAsXmlString()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-16\"?><Location xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Country>United States</Country><State>California</State><ZipCode>90210</ZipCode><Places><Place><Name>Sun City</Name><Inhabitants>100000</Inhabitants><IsCapital>true</IsCapital></Place><Place><Name>Pleasure Meadow</Name><Inhabitants>50000</Inhabitants><IsCapital>false</IsCapital></Place></Places></Location>";
        }

        /// <summary>
        /// Returns a sample HTML response body as a string.
        /// </summary>
        /// <returns>A sample HTML response body as a string.</returns>
        protected string GetHtmlResponseBody()
        {
            return "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\"/><title>403 - Forbidden: Access is denied.</title></head><body><div id=\"header\"><h1>Server Error</h1></div><div id=\"content\"><div class=\"content-container\"><fieldset><h2>403 - Forbidden: Access is denied.</h2><h3>You do not have permission to view this directory or page using the credentials that you supplied.</h3></fieldset></div></div></body></html>";
        }
    }
}
