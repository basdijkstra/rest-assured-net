// <copyright file="RequestBodyUsageExamples.cs" company="On Test Automation">
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
using NUnit.Framework;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestBodyUsageExamples
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for sending
        /// a request body as a string when performing an HTTP POST.
        /// </summary>
        [Test]
        public void PostANewPost_CheckHttpStatusCode_ShouldBe201()
        {
            Given()
            .Body("{\"userId\": 1, \"title\": \"My post title\", \"body\": \"My post body\"}")
            .When()
            .Post("https://jsonplaceholder.typicode.com/posts")
            .Then()
            .StatusCode(201);
        }
    }
}