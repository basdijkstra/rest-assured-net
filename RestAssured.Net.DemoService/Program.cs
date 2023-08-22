// <copyright file="Program.cs" company="On Test Automation">
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
namespace DemoService
{
    /// <summary>
    /// Class containing the implementation of our demo service.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method invoked when starting the demo service.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
            };

            app.MapGet("/weatherforecast", () =>
            {
                return new WeatherForecast(
                        DateTime.Now.AddDays(1),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]);
            });

            app.Run();
        }
    }

    internal record WeatherForecast(DateTime date, int temperatureC, string? summary)
    {
        /// <summary>
        /// The temperate in degrees Fahrenheit.
        /// </summary>
        public int TemperatureF => 32 + (int)(this.temperatureC / 0.5556);
    }
}