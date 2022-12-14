# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

### [Unreleased]

### [2.2.1] - 2023-01-10

#### Fixed

- Fixed an issue with incorrect URL encoding when adding query parameters to a relative URL ([#47](https://github.com/basdijkstra/rest-assured-net/issues/47) by [@MuresanCristianRichard](https://github.com/MuresanCristianRichard))

### [2.2.0] - 2022-12-31

#### Added

- Added support for specifying request and response log level through static RestAssured configuration ([#43](https://github.com/basdijkstra/rest-assured-net/issues/43) by [@Ukrainis](https://github.com/Ukrainis))
- Added support for specifying request log level through the RequestSpecification ([#44](https://github.com/basdijkstra/rest-assured-net/issues/44))
- Added `Log(RequestLogLevel requestLogLevel)` method for request logging ([#46](https://github.com/basdijkstra/rest-assured-net/issues/46))
- Added `Log(ResponseLogLevel responseLogLevel)` method for response logging ([#46](https://github.com/basdijkstra/rest-assured-net/issues/46))

#### Updated

- Version bumps for Newtonsoft.Json (main project) and WireMock.Net and Microsoft.NET.Test.Sdk (test project)

#### Deprecated

- Deprecated `Log().All()` and other methods for request logging in favour of `Log(RequestLogLevel requestLogLevel)`.
- Deprecated `Log().All()` and other methods for response logging in favour of `Log(ResponseLogLevel responseLogLevel)`.

### [2.1.0] - 2022-11-30

#### Added

- Added support for sending simple and parameterized GraphQL queries using a GraphQLRequest object and builder ([#38](https://github.com/basdijkstra/rest-assured-net/issues/38))
- Added support for global configuration of SSL check disabling using RestAssuredConfig ([#40](https://github.com/basdijkstra/rest-assured-net/issues/38) by [@mennopot](https://github.com/mennopot))

#### Fixed

- Fixed missing documentation for query and path parameters in Usage Guide ([#42](https://github.com/basdijkstra/rest-assured-net/issues/42))

#### Updated

- Version bumps for NHamcrest (main project) and NUnit3TestAdapter (test project)

### [2.0.0] - 2022-11-20

#### Changed

- (BREAKING CHANGE) To allow for easier understanding and extending of the code, the class and namespace structure has changed to follow a 'folder-by-feature' structure. Please use `using static RestAssured.Dsl;` to start writing tests with RestAssured.Net from this release onwards.

Thanks to [@appie2go](https://github.com/appie2go) for PRs [#34](https://github.com/basdijkstra/rest-assured-net/pull/34) and [#37](https://github.com/basdijkstra/rest-assured-net/pull/37), which contain quite a few improvements to the code, as well as for your other suggestions on improving the code structure.

### [1.2.0] - 2022-11-17

#### Added

- Added support for configuring the ignoring of SSL errors in a RequestSpecification ([#31](https://github.com/basdijkstra/rest-assured-net/issues/31))
- Added the option to log the response time to the console ([#32](https://github.com/basdijkstra/rest-assured-net/issues/32) by [@bheemreddy181](https://github.com/bheemreddy181))
- Added the option to log request details to the console ([#33](https://github.com/basdijkstra/rest-assured-net/issues/33) by [@bheemreddy181](https://github.com/bheemreddy181))

#### Fixed

- Fixed an issue with relative paths being incorrectly converted to file paths on Unix and MacOS ([#22](https://github.com/basdijkstra/rest-assured-net/issues/22) by [@bheemreddy181](https://github.com/bheemreddy181))

### [1.1.1] - 2022-11-12

#### Added 

- Added support for ignoring SSL errors ([#13](https://github.com/basdijkstra/rest-assured-net/issues/13) by [@bheemreddy181](https://github.com/bheemreddy181))
- Added support for specifying Content-Type and content encoding in a RequestSpecification ([#24](https://github.com/basdijkstra/rest-assured-net/issues/24))
- Added support for specifying headers (including Basic and OAuth2 authorization) in a RequestSpecification ([#11](https://github.com/basdijkstra/rest-assured-net/issues/11) by [@bheemreddy181](https://github.com/bheemreddy181))
- Added .NET 7 to the list of target frameworks ([#21](https://github.com/basdijkstra/rest-assured-net/issues/21))
- Added support for specifying a user agent for individual requests as well as in a RequestSpecification ([#16](https://github.com/basdijkstra/rest-assured-net/issues/16) by [@bheemreddy181](https://github.com/bheemreddy181))
- Added support for specifying custom timeouts for individual requests as well as in a RequestSpecification ([#15](https://github.com/basdijkstra/rest-assured-net/issues/15) by [@bheemreddy181](https://github.com/bheemreddy181))
- Added support for sending x-www-form-urlencoded data in requests ([#6](https://github.com/basdijkstra/rest-assured-net/issues/6) by [@bheemreddy181](https://github.com/bheemreddy181))
- Added support for validating JSON response payloads against a JSON schema ([#8](https://github.com/basdijkstra/rest-assured-net/issues/8))

#### Fixed

- Fixed an issue with port numbers defaulting to 80 even when scheme was set to HTTPS ([#25](https://github.com/basdijkstra/rest-assured-net/issues/25))
- Fixed an issue where trying to log a response with an empty response body would throw a NullReferenceException ([#26](https://github.com/basdijkstra/rest-assured-net/issues/26))

### [1.0.0] - 2022-11-03

#### Added

- First public, non-alpha/-beta version of RestAssured.Net 