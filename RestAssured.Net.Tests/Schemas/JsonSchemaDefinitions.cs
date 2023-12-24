// <copyright file="JsonSchemaDefinitions.cs" company="On Test Automation">
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
namespace RestAssured.Tests.Schemas
{
    /// <summary>
    /// A class containing JSON schema definitions used in the tests.
    /// </summary>
    public class JsonSchemaDefinitions
    {
        /// <summary>
        /// A JSON schema that matches the JSON payload used in the tests.
        /// </summary>
        internal static string MatchingJsonSchema { get; } = @"{ 'type': 'object', 'properties': { 'name': { 'type':'string'}, 'hobbies': { 'type': 'array', 'items': { 'type': 'string' } } } }";

        /// <summary>
        /// A JSON schema that is not correctly formatted.
        /// </summary>
        internal static string InvalidJsonSchemaAsString { get; } = @"{ 'object', 'properties': { 'name': { 'type':'string'}, 'hobbies': { 'type': 'array', 'items': { 'type': 'string' } } } }";
    }
}
