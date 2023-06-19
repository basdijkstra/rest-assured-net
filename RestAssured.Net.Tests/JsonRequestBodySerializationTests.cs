// <copyright file="JsonRequestBodySerializationTests.cs" company="On Test Automation">
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
    using NUnit.Framework;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class JsonRequestBodySerializationTests : TestBase
    {
        private readonly string expectedSerializedJsonRequestBody = "{\"Country\":\"United States\",\"State\":\"California\",\"ZipCode\":90210,\"Places\":[{\"Name\":\"Sun City\",\"Inhabitants\":100000,\"IsCapital\":true},{\"Name\":\"Pleasure Meadow\",\"Inhabitants\":50000,\"IsCapital\":false}]}";
        private readonly string expectedSerializedObject = "{\"Id\":1,\"Title\":\"My post title\",\"Body\":\"My post body\"}";
        
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// and sending a JSON request body when performing an HTTP POST.
        /// </summary>
        [Test]
        public void ObjectCanBeSerializedToJson()
        {
            this.CreateStubForJsonRequestBody();

            Given()
                .Body(this.GetLocation())
                .When()
                .Post("http://localhost:9876/json-serialization")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// a Dictionary to JSON and sending it when performing an HTTP POST.
        /// </summary>
        [Test]
        public void DictionaryCanBeSerializedToJson()
        {
            this.CreateStubForObjectSerialization();

            Dictionary<string, object> post = new Dictionary<string, object>
            {
                { "Id", 1 },
                { "Title", "My post title" },
                { "Body", "My post body" },
            };

            Given()
                .Body(post)
                .When()
                .Post("http://localhost:9876/object-serialization")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// an anonymous type to JSON and sending it when performing an HTTP POST.
        /// </summary>
        [Test]
        public void AnonymousObjectCanBeSerializedToJson()
        {
            this.CreateStubForObjectSerialization();

            var post = new
            {
                Id = 1,
                Title = "My post title",
                Body = "My post body",
            };

            Given()
                .Body(post)
                .When()
                .Post("http://localhost:9876/object-serialization")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// Creates the stub response for the JSON string request body example.
        /// </summary>
        private void CreateStubForJsonRequestBody()
        {
            this.Server?.Given(Request.Create().WithPath("/json-serialization").UsingPost()
                .WithBody(new JsonMatcher(this.expectedSerializedJsonRequestBody)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }

        /// <summary>
        /// Creates the stub response for the object serialization example.
        /// </summary>
        private void CreateStubForObjectSerialization()
        {
            this.Server?.Given(Request.Create().WithPath("/object-serialization").UsingPost()
                .WithBody(new JsonMatcher(this.expectedSerializedObject)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}