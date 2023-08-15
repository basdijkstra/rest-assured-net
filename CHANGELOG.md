# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

### Unreleased

Nothing yet.

### [4.1.0] - 2023-08-15

#### Added

- Added support for supplying custom `System.Net.Http.HttpClient` to be used instead of the one created by RestAssured .Net ([#103](https://github.com/basdijkstra/rest-assured-net/issues/103) by [@JoeBatt1989](https://github.com/JoeBatt1989))
- Added support for supplying multiple query parameter values ([#105](https://github.com/basdijkstra/rest-assured-net/issues/105) by [@lorszi456](https://github.com/lorszi456))

#### Fixed

- Improved logging of empty request and response bodies ([#104](https://github.com/basdijkstra/rest-assured-net/issues/104))

#### Updated

- Version bumps for HtmlAgilityPack, NHamcrest (main project), Microsoft.NET.Test.Sdk, NUnit3TestAdapter and WireMock.NET (test project)

### [4.0.0] - 2023-07-11

#### Added

- Added ability to mask sensitive values for request and response headers and cookies in logging ([#93](https://github.com/basdijkstra/rest-assured-net/issues/93))

#### Fixed

- Fixed issue with request cookies not being logged ([#98](https://github.com/basdijkstra/rest-assured-net/issues/98))

#### Changed

- (BREAKING CHANGE) Replaced `Newtonsoft.Json.Schema` with `NJsonSchema` due to restrictive licensing of the former ([#96](https://github.com/basdijkstra/rest-assured-net/issues/96) by [@aaschmid](https://github.com/aaschmid))

### [3.0.0] - 2023-07-04

#### Added

- Added ability to verify response cookie values ([#90](https://github.com/basdijkstra/rest-assured-net/issues/90) by [@workmichsem](https://github.com/workmichsem))
- Added ability to extract response cookie values as a string ([#90](https://github.com/basdijkstra/rest-assured-net/issues/90) by [@workmichsem](https://github.com/workmichsem))
- Added logging of response cookie details when using `ResponseLogLevel.All` or `ResponseLogLevel.Headers` ([#90](https://github.com/basdijkstra/rest-assured-net/issues/90) by [@workmichsem](https://github.com/workmichsem))
- Added `Invoke()` method allowing you to easily make the same call with different HTTP methods ([#82](https://github.com/basdijkstra/rest-assured-net/issues/82))
- Added ability to specify custom settings for deserializing request payloads from JSON in individual tests ([#84](https://github.com/basdijkstra/rest-assured-net/issues/84))
- Added ability to specify custom settings for serializing request payloads to JSON in individual tests and through a RequestSpecification ([#84](https://github.com/basdijkstra/rest-assured-net/issues/84))
- Added ability to verify the response time using an NHamcrest matcher ([#91](https://github.com/basdijkstra/rest-assured-net/issues/91) by [@christiaanwvermaak](https://github.com/christiaanwvermaak))
- Added ability to extract the response time into a `TimeSpan` ([#91](https://github.com/basdijkstra/rest-assured-net/issues/91) by [@christiaanwvermaak](https://github.com/christiaanwvermaak))
- Added ability to verify the response body length using an NHamcrest matcher ([#92](https://github.com/basdijkstra/rest-assured-net/issues/92) by [@christiaanwvermaak](https://github.com/christiaanwvermaak))
- Added ability to extract the entire response body as a string ([#92](https://github.com/basdijkstra/rest-assured-net/issues/92) by [@christiaanwvermaak](https://github.com/christiaanwvermaak))

#### Changed

- (BREAKING CHANGE) Changed delimiters for path parameter placeholders from `{{` and `}}` to `[` and `]`, respectively, to enable path parameter usage in combination with C# string interpolation ([#89](https://github.com/basdijkstra/rest-assured-net/issues/89))

#### Removed

- (BREAKING CHANGE) Removed support for .NET Core 3.1
- Removed `As()` for deserializing response payloads in favour of `DeserializeTo()`
- Removed `As()` and `DeserializeTo()` methods in `ExtractableResponse` in favour of deserializing response in `VerifiableResponse` to clean up the library API
- Removed `UseRelaxedHttpsValidation()` and other methods and properties for disabling SSL certificate checks in favour of `DisableSslCertificateValidation()` ([#79](https://github.com/basdijkstra/rest-assured-net/issues/79))
- Removed `Log().All()` and other methods for request logging in favour of `Log(RequestLogLevel requestLogLevel)`.
- Removed `Log().All()` and other methods for response logging in favour of `Log(ResponseLogLevel responseLogLevel)`.

### [2.8.1] - 2023-05-23

#### Fixed

- Fixed an issue with server-side certificates not being properly validated ([#77](https://github.com/basdijkstra/rest-assured-net/issues/77) by [@roydekleijn](https://github.com/roydekleijn))

#### Deprecated

- Deprecated `UseRelaxedHttpsValidation()` and other methods and properties for disabling SSL certificate checks in favour of `DisableSslCertificateValidation()` ([#79](https://github.com/basdijkstra/rest-assured-net/issues/79))

#### Updated

- Version bump for Microsoft.NET.Test.Sdk (test project)

### [2.8.0] - 2023-05-15

#### Added

- Added ability to override automatically determined evaluator (based on response Content-Type header) when verifying response body elements
 ([#75](https://github.com/basdijkstra/rest-assured-net/issues/75))
- Added ability to override automatically determined evaluator (based on response Content-Type header) when extracting response body elements
 ([#76](https://github.com/basdijkstra/rest-assured-net/issues/76))
 
#### Updated

- Version bump for WireMock.Net (test project)

### [2.7.0] - 2023-05-10

#### Added

- Added ability to override automatically determined deserializer ([#73](https://github.com/basdijkstra/rest-assured-net/issues/73))

### [2.6.0] - 2023-05-08

#### Added

- Added support for verifying HTML response body elements ([#70](https://github.com/basdijkstra/rest-assured-net/issues/70) by [@workmichsem](https://github.com/workmichsem))
- Added support for extracting HTML response body elements ([#70](https://github.com/basdijkstra/rest-assured-net/issues/70) by [@workmichsem](https://github.com/workmichsem))
- Added the ability to validate XML response payloads against an XML schema ([#65](https://github.com/basdijkstra/rest-assured-net/issues/65))
- Added the ability to validate XML response payloads against an inline DTD ([#65](https://github.com/basdijkstra/rest-assured-net/issues/65))
- Added the ability to log response details only when the response status code indicates an error (i.e., 4xx or 5xx) ([#64](https://github.com/basdijkstra/rest-assured-net/issues/64))
- Added the ability to log response details only when a response verification fails ([#63](https://github.com/basdijkstra/rest-assured-net/issues/63))
- Added the `DeserializeTo()` method alias for response body deserialization

#### Removed

- Removed the `SkipNet7` environment parameter used to skip running tests on .NET 7 ([#69](https://github.com/basdijkstra/rest-assured-net/issues/69))

#### Updated

- Version bumps for Newtonsoft.Json (main project) and WireMock.Net (test project)

### [2.5.0] - 2023-04-03

#### Added

- Added improved multipart file upload, including the ability to overwrite the automatically determined MIME type and uploading multiple files in a single HTTP call ([#58](https://github.com/basdijkstra/rest-assured-net/issues/58) by [@MuresanCristianRichard](https://github.com/MuresanCristianRichard))

#### Fixed

- Fixed issue with Content-Length and other content headers not being included when logging requests and responses ([#59](https://github.com/basdijkstra/rest-assured-net/issues/59) by [@workmichsem](https://github.com/workmichsem))

#### Updated

- Version bump for WireMock.Net (test project)

### [2.4.0] - 2023-03-20

#### Added

- Added support for HEAD and OPTIONS HTTP verbs ([#55](https://github.com/basdijkstra/rest-assured-net/issues/55))
- Added the ability to deserialize the response body after initial verifications in the `Then()` section of a test ([#54](https://github.com/basdijkstra/rest-assured-net/issues/54))

#### Updated

- Version bumps for Newtonsoft.Json (main project) and WireMock.Net and NUnit3TestAdapter (test project)

### [2.3.0] - 2023-03-02

#### Added

- Added support for uploading multipart files ([#53](https://github.com/basdijkstra/rest-assured-net/issues/53) by [@RipaBogdan](https://github.com/RipaBogdan))

#### Updated

- Version bumps for WireMock.Net and Microsoft.NET.Test.Sdk (test project)

### [2.2.2] - 2023-01-31

#### Updated

- Migrated CI from CircleCI to GitHub Actions ([#48](https://github.com/basdijkstra/rest-assured-net/pull/48) by [@drakulavich](https://github.com/drakulavich))
- Improved code documentation, code styling and variable names (all reported by [@dev-experience](https://github.com/dev-experience))

#### Fixed

- Fixed a vulnerability issue in System.Text.Encodings.Web (transitive dependency from Microsoft.AspNetCore.WebUtilities) by explicitly adding a safe version as a dependency (reported by [@dev-experience](https://github.com/dev-experience))
- Fixed lots of nullability warnings and (all reported by [@dev-experience](https://github.com/dev-experience))

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
