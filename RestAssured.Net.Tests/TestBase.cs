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
    using NUnit.Framework;
    using RestAssured.Tests.Models;
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

        protected static readonly int MOCK_SERVER_PORT = 9876;

        protected static readonly string MOCK_SERVER_BASE_URL = $"http://localhost:{MOCK_SERVER_PORT}";

        /// <summary>
        /// Starts the WireMock server before every test.
        /// </summary>
        [SetUp]
        protected void StartServer()
        {
            this.Server = WireMockServer.Start(MOCK_SERVER_PORT);
        }

        /// <summary>
        /// Stops the WireMock server after every test.
        /// </summary>
        [TearDown]
        protected void StopServer()
        {
            this.Server?.Stop();
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
