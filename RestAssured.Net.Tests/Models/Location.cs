// <copyright file="Location.cs" company="On Test Automation">
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
namespace RestAssured.Tests.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A POCO representing a location on earth.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The country for the country code and zip code.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// The state for the country code and zip code.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The zip code for the country code and zip code.
        /// </summary>
        public int ZipCode { get; set; }

        /// <summary>
        /// The list of places associated with the country code and zip code.
        /// </summary>
        public List<Place> Places { get; set; }

        public Location()
        {
            this.Places = new List<Place>();
        }
    }
}
