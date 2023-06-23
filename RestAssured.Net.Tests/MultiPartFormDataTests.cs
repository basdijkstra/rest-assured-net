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

        private string[] addressItems;

        /// <summary>
        /// Creates the files to be uploaded in these tests.
        /// </summary>
        /// <returns>The asynchronous test result.</returns>
        [SetUp]
        public async Task CreateFilesToUpload()
        {
            this.addressItems = this.GetAddressCsv(Faker.RandomNumber.Next(2, 8));
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
                .Post($"{MOCK_SERVER_BASE_URL}/plaintext-multipart-form-data")
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
                .Post($"{MOCK_SERVER_BASE_URL}/csv-multipart-form-data")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// multiple files when submitting multipart form data
        /// with the default 'file' control name and an automatically
        /// determined content type in the request.
        /// </summary>
        [Test]
        public void MultipleFilesCanBeSupplied()
        {
            this.CreateStubForMultipleFilesMultiPartFormData();

            Given()
                .MultiPart(new FileInfo(this.plaintextFileName))
                .MultiPart(new FileInfo(this.csvFileName), "customControl", MediaTypeHeaderValue.Parse("text/csv"))
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/multiple-files-multipart-form-data")
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
                .MultiPart(new FileInfo(@"DoesNotExist.txt"))
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/plaintext-multipart-form-data")
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

        private string GetAddressCsvLine()
        {
            return string.Format(
                "{0};{1};{2};{3}",
                Faker.Address.StreetName(),
                Faker.RandomNumber.Next(1, 9999),
                Faker.Address.ZipCode(),
                Faker.Address.City());
        }

        private string[] GetAddressCsv(int lines)
        {
            string[] csvLines = new string[lines];
            csvLines[0] = "Street;Number;ZipCode;City";
            for (int i = 1; i < lines; i++)
            {
                csvLines[i] = this.GetAddressCsvLine();
            }

            return csvLines;
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

        /// <summary>
        /// Creates the stub response for the csv form data example.
        /// </summary>
        private void CreateStubForMultipleFilesMultiPartFormData()
        {
            this.Server?.Given(Request.Create().WithPath("/multiple-files-multipart-form-data").UsingPost()
                .WithHeader("Content-Type", new RegexMatcher("multipart/form-data; boundary=.*"))
                .WithBody(new RegexMatcher($".*ToDoItems.*"))
                .WithBody(new RegexMatcher($".*Addresses.*")))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}