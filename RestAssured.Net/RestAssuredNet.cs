﻿// <copyright file="RestAssuredNet.cs" company="On Test Automation">
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

using RestAssured.Net.RA;
using RestAssuredNet.RA;

namespace RestAssuredNet
{
    /// <summary>
    /// Entry point to the RestAssuredNet code and writing tests for HTTP-based APIs.
    /// </summary>
    public class RestAssuredNet
    {
        /// <summary>
        /// Used to start writing a new test.
        /// </summary>
        /// <returns>A <see cref="ExecutableRequest"/> object containing all relevant request properties.</returns>
        public static ExecutableRequest Given()
        {
            return new ExecutableRequest();
        }
    }
}