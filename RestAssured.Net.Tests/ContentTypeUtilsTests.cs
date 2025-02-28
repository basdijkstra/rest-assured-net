// <copyright file="ContentTypeUtilsTests.cs" company="On Test Automation">
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
    using RestAssured.Response;
    using RestAssured.Response.ContentType;
    using RestAssured.Response.Exceptions;

    /// <summary>
    /// Tests for the <see cref="ContentTypeUtils"/> class.
    /// </summary>
    [TestFixture]
    public class ContentTypeUtilsTests
    {
        private ContentTypeUtils utils;

        /// <summary>
        /// Set up the initial state for the tests.
        /// </summary>
        [SetUp]
        public void CreateNewUtilsClass()
        {
            this.utils = new ContentTypeUtils();
        }

        /// <summary>
        /// Verifies that an unsupported value for a Content-Type header is ignored when setting a specific VerifyAs value.
        /// </summary>
        /// <param name="verifyAs">The <see cref="VerifyAs"/> value to use in the test.</param>
        /// <param name="supportedContentType">The <see cref="SupportedContentType"/> value to use in the test.</param>
        [TestCase(VerifyAs.Json, SupportedContentType.Json, TestName = "Unsupported Content-Type header is ignored when verifying as JSON")]
        [TestCase(VerifyAs.Xml, SupportedContentType.Xml, TestName = "Unsupported Content-Type header is ignored when verifying as XML")]
        [TestCase(VerifyAs.Html, SupportedContentType.Html, TestName = "Unsupported Content-Type header is ignored when verifying as HTML")]
        public void InvalidValueIsIgnoredWhenVerifyingAsSpecifiedValue(VerifyAs verifyAs, SupportedContentType supportedContentType)
        {
            var contentType = this.utils.DetermineResponseMediaTypeForResponse("application/banana", verifyAs);

            Assert.That(contentType, Is.EqualTo(supportedContentType));
        }

        /// <summary>
        /// Verifies that an unsupported value for a Content-Type header is ignored when setting a specific ExtractAs value.
        /// </summary>
        /// <param name="extractAs">The <see cref="ExtractAs"/> value to use in the test.</param>
        /// <param name="supportedContentType">The <see cref="SupportedContentType"/> value to use in the test.</param>
        [TestCase(ExtractAs.Json, SupportedContentType.Json, TestName = "Unsupported Content-Type header is ignored when extracting as JSON")]
        [TestCase(ExtractAs.Xml, SupportedContentType.Xml, TestName = "Unsupported Content-Type header is ignored when extracting as XML")]
        [TestCase(ExtractAs.Html, SupportedContentType.Html, TestName = "Unsupported Content-Type header is ignored when extracting as HTML")]
        public void InvalidValueIsIgnoredWhenExtractingAsSpecifiedValue(ExtractAs extractAs, SupportedContentType supportedContentType)
        {
            var contentType = this.utils.DetermineResponseMediaTypeForResponse("application/banana", extractAs);

            Assert.That(contentType, Is.EqualTo(supportedContentType));
        }

        /// <summary>
        /// Verifies that an empty value for a Content-Type header defaults to JSON.
        /// </summary>
        [Test]
        public void EmptyValueDefaultsToJsonWhenDeterminingMediaResponseType()
        {
            var contentType = this.utils.DetermineResponseMediaTypeForResponse(string.Empty, ExtractAs.UseResponseContentTypeHeaderValue);

            Assert.That(contentType, Is.EqualTo(SupportedContentType.Json));
        }

        /// <summary>
        /// Verifies that an unsupported value for a Content-Type header throws an ExtractionException.
        /// </summary>
        [Test]
        public void InvalidValueThrowsExtractionExceptionWhenDeterminingMediaResponseType()
        {
            string invalidMediaType = "application/banana";

            var ee = Assert.Throws<ExtractionException>(() =>
            {
                this.utils.DetermineResponseMediaTypeForResponse(invalidMediaType, ExtractAs.UseResponseContentTypeHeaderValue);
            });

            Assert.That(ee.Message, Is.EqualTo($"Unable to extract elements from response with Content-Type '{invalidMediaType}'"));
        }
    }
}
