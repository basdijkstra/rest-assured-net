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
    /// A POCO representing a blog post.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The id of the blog post.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the user.
        /// </summary>
        public string Name { get; set; }

        public User(){
            this.Id = Faker.RandomNumber.Next(99999);
            this.Name = Faker.Name.FullName();
        }

        public string getJsonString(){
            return "{\"id\":" + this.Id + 
                ",\"user\":\"" + this.Name + "\"}";
        }
    }
}