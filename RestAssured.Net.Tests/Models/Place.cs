// <copyright file="Place.cs" company="On Test Automation">
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
    /// <summary>
    /// A POCO representing a place on earth.
    /// </summary>
    public class Place
    {
        /// <summary>
        /// The name of the place.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The number of inhabitants of the place.
        /// </summary>
        public int Inhabitants { get; set; }

        /// <summary>
        /// Indication whether or not the place is the capital of a region.
        /// </summary>
        public bool IsCapital { get; set; }

        public Place()
        {
            this.Name = Faker.Address.City();
            this.Inhabitants = Faker.RandomNumber.Next(10000, 99999999);
            this.IsCapital = Faker.Boolean.Random();
        }
    }
}
