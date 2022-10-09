// <copyright file="RestAssuredNetUsageExamples.cs" company="On Test Automation">
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
    public class RestAssuredNetUsageExamples
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP GET.
        /// </summary>
        [Test]
        public void GetDataForUsZipCode90210_CheckHttpStatusCode_ShouldBe200()
        {
            Given()
            .When()
            .Get("http://api.zippopotam.us/us/90210")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP DELETE.
        /// </summary>
        [Test]
        public void DeleteDataForPost1_CheckHttpStatusCode_ShouldBe200()
        {
            Given()
            .When()
            .Delete("https://jsonplaceholder.typicode.com/posts/1")
            .Then()
            .StatusCode(200);
        }
    }
}