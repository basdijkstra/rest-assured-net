// <copyright file="RequestExceptionTests.cs" company="On Test Automation">
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
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RestAssured.Logging;
    using RestAssured.Request.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Tests checking that exceptions thrown while sending a request are reported with their original cause.
    /// </summary>
    [TestFixture]
    public class RequestExceptionTests : TestBase
    {
        /// <summary>
        /// A test checking that an <see cref="HttpRequestException"/> (for example a refused connection)
        /// is reported with its original message, and with the original exception as the inner exception.
        /// </summary>
        [Test]
        public void HttpRequestExceptionIsReportedWithOriginalMessageAndInnerException()
        {
            var httpRequestException = new HttpRequestException("Connection refused");
            using HttpClient httpClient = new HttpClient(new ThrowingHandler(httpRequestException));

            var hrpe = Assert.Throws<HttpRequestProcessorException>(() =>
                Given(httpClient)
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/does-not-matter"));

            Assert.That(hrpe?.Message, Is.EqualTo("Connection refused"));
            Assert.That(hrpe?.InnerException, Is.SameAs(httpRequestException));
        }

        /// <summary>
        /// A test checking that an unexpected exception thrown while sending a request
        /// is reported with the original exception as the inner exception.
        /// </summary>
        [Test]
        public void UnexpectedExceptionIsReportedWithInnerException()
        {
            var invalidOperationException = new InvalidOperationException("boom");
            using HttpClient httpClient = new HttpClient(new ThrowingHandler(invalidOperationException));

            var hrpe = Assert.Throws<HttpRequestProcessorException>(() =>
                Given(httpClient)
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/does-not-matter"));

            Assert.That(hrpe?.Message, Is.EqualTo("Unhandled exception boom"));
            Assert.That(hrpe?.InnerException, Is.SameAs(invalidOperationException));
        }

        /// <summary>
        /// A test checking that an exception thrown by a custom logger while logging the response
        /// is not reported as an <see cref="HttpRequestProcessorException"/>, but passed on unchanged.
        /// </summary>
        [Test]
        public void ExceptionThrownByLoggerIsNotWrapped()
        {
            this.Server?.Given(Request.Create().WithPath("/logger-exception").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));

            Assert.Throws<NotSupportedException>(() =>
                Given(new ThrowingLogger())
                    .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.All })
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/logger-exception"));
        }

        /// <summary>
        /// An <see cref="HttpMessageHandler"/> that throws the specified exception for every request.
        /// </summary>
        private class ThrowingHandler : HttpMessageHandler
        {
            private readonly Exception exception;

            /// <summary>
            /// Initializes a new instance of the <see cref="ThrowingHandler"/> class.
            /// </summary>
            /// <param name="exception">The exception to throw when a request is sent.</param>
            public ThrowingHandler(Exception exception)
            {
                this.exception = exception;
            }

            /// <inheritdoc/>
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw this.exception;
            }
        }

        /// <summary>
        /// An <see cref="IRestAssuredNetLogger"/> that throws a <see cref="NotSupportedException"/> for every message.
        /// </summary>
        private class ThrowingLogger : IRestAssuredNetLogger
        {
            /// <inheritdoc/>
            public void Log(string message)
            {
                throw new NotSupportedException("This logger does not support logging.");
            }
        }
    }
}
