// <copyright file="MultiPartFormDataTests.cs" company="On Test Automation">
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
    using System.IO;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RestAssured.Request.Exceptions;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class MultiPartFormDataTests : TestBase
    {
        private readonly string plaintextFileName = @"ToDoItems.txt";
        private readonly string csvFileName = @"Addresses.csv";

        private readonly string todoItem = "Watch Office Space";

        private readonly string[] addressItems = new string[]
        {
            "Street;Number;ZipCode;City",
            "Main Street;123;12345;Nothingville",
            "State Street;987;23456;Sun City",
        };

        /// <summary>
        /// Creates the files to be uploaded in these tests.
        /// </summary>
        /// <returns>The asynchronous test result.</returns>
        [SetUp]
        public async Task CreateFilesToUpload()
        {
            await File.WriteAllLinesAsync(this.plaintextFileName, new string[] { this.todoItem });
            await File.WriteAllLinesAsync(this.csvFileName, this.addressItems);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// multipart form data with the default 'file' control name
        /// and an automatically determined content type in the request.
        /// </summary>
        [Test]
        public void MultiPartFormDataWithDefaultControlNameAndAutoDetectedContentTypeCanBeSupplied()
        {
            this.CreateStubForPlainTextMultiPartFormData();

            Given()
                .MultiPart(new FileInfo(this.plaintextFileName))
                .When()
                .Post("http://localhost:9876/plaintext-multipart-form-data")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// multipart form data with a custom control name and a custom
        /// content type in the request.
        /// </summary>
        [Test]
        public void MultiPartFormDataWithCustomControlNameAndCustomContentTypeCanBeSupplied()
        {
            this.CreateStubForCsvMultiPartFormData();

            Given()
                .MultiPart(new FileInfo(this.csvFileName), "customControl", MediaTypeHeaderValue.Parse("text/csv"))
                .When()
                .Post("http://localhost:9876/csv-multipart-form-data")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that trying to upload a nonexistent file throws the expected
        /// exception.
        /// </summary>
        [Test]
        public void UploadingNonExistentFileThrowsTheExpectedException()
        {
            this.CreateStubForPlainTextMultiPartFormData();

            var rce = Assert.Throws<RequestCreationException>(() =>
            {
                Given()
                .MultiPart("customControl", @"DoesNotExist.txt")
                .When()
                .Post("http://localhost:9876/plaintext-multipart-form-data")
                .Then()
                .StatusCode(201);
            });

            Assert.That(rce?.Message, Does.Contain("Could not find file"));
        }

        /// <summary>
        /// Deletes the file created for test execution.
        /// </summary>
        [TearDown]
        public void DeleteFile()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Delete(this.plaintextFileName);
            File.Delete(this.csvFileName);
        }

        /// <summary>
        /// Creates the stub response for the plaintext form data example.
        /// </summary>
        private void CreateStubForPlainTextMultiPartFormData()
        {
            this.Server?.Given(Request.Create().WithPath("/plaintext-multipart-form-data").UsingPost()
                .WithHeader("Content-Type", new RegexMatcher("multipart/form-data; boundary=.*"))
                .WithBody(new RegexMatcher($".*text/plain.*"))
                .WithBody(new RegexMatcher($".*name=file.*")))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }

        /// <summary>
        /// Creates the stub response for the csv form data example.
        /// </summary>
        private void CreateStubForCsvMultiPartFormData()
        {
            this.Server?.Given(Request.Create().WithPath("/csv-multipart-form-data").UsingPost()
                .WithHeader("Content-Type", new RegexMatcher("multipart/form-data; boundary=.*"))
                .WithBody(new RegexMatcher($".*text/csv.*"))
                .WithBody(new RegexMatcher($".*name=customControl.*")))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}