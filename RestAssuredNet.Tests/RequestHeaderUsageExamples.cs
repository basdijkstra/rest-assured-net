// <copyright file="RequestHeaderUsageExamples.cs" company="On Test Automation">
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
using System.Collections.Generic;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestHeaderUsageExamples
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a header with a single value when sending an HTTP request.
        /// </summary>
        [Test]
        public void AddHeaderWithASingleValue()
        {
            Given()
            .Header("my_header", "my_header_value")
            .When()
            .Get("https://jsonplaceholder.typicode.com/posts")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a header with multiple values when sending an HTTP request.
        /// </summary>
        [Test]
        public void AddHeaderWithMultipleValues()
        {
            Given()
            .Header("my_header", new List<string>() { "my_header_value_1", "my_header_value_2" })
            .When()
            .Get("https://jsonplaceholder.typicode.com/posts")
            .Then()
            .StatusCode(200);
        }
    }
}