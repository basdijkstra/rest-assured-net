// <copyright file="RequestBodySettings.cs" company="On Test Automation">
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
namespace RestAssured.Request
{
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Holds the serialization-related settings for creating a request body.
    /// </summary>
    /// <param name="ContentType">The resolved content type for the request.</param>
    /// <param name="ContentEncoding">The resolved content encoding for the request.</param>
    /// <param name="StripCharset">Flag indicating whether the charset should be stripped from the content type.</param>
    /// <param name="JsonSerializerSettings">The resolved JSON serializer settings for the request.</param>
    /// <param name="ContentTypeSetInRequest">Flag indicating whether the content type was set in the request itself (and not only in a request specification).</param>
    internal record RequestBodySettings(
        string ContentType,
        Encoding ContentEncoding,
        bool StripCharset,
        JsonSerializerSettings JsonSerializerSettings,
        bool ContentTypeSetInRequest);
}
