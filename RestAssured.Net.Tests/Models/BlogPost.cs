// <copyright file="BlogPost.cs" company="On Test Automation">
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
    public class BlogPost
    {
        /// <summary>
        /// The id of the blog post.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The title of the blog post.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The number of inhabitants of the place.
        /// </summary>
        public string Body { get; set; }

        public BlogPost()
        {
            this.Id = Faker.RandomNumber.Next();
            this.Title = Faker.Lorem.Sentence(5);
            this.Body = Faker.Lorem.Sentence(Faker.RandomNumber.Next(10, 20));
        }

        public string GetSerializedJson()
        {
            return "{\"Id\":" + this.Id +
                ",\"Title\":\"" + this.Title +
                "\",\"Body\":\"" + this.Body + "\"}";
        }
    }
}