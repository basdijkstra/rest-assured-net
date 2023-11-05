// <copyright file="ReturnAs.cs" company="On Test Automation">
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

namespace RestAssured.Response
{
    /// <summary>
    /// Contains the different options for forcing returning the value in a specific format.
    /// </summary>
    public enum ReturnAs
    {
        /// <summary>
        /// Indicates returning singular extraction results as a singular value.
        /// </summary>
        Singular = 0,

        /// <summary>
        /// Indicates returning singular extraction results as a list with a single item.
        /// </summary>
        List = 1,
    }
}
