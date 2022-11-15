# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

### [Unreleased]

#### Added

- Added support for configuring the ignoring of SSL errors in a RequestSpecification ([#31](https://github.com/basdijkstra/rest-assured-net/issues/31))

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

- Fixed an issue with port numbers defaulting to 80 even when scheme was set to HTTPS ([#26](https://github.com/basdijkstra/rest-assured-net/issues/26))
- Fixed an issue where trying to log a response with an empty response body would throw a NullReferenceException ([#26](https://github.com/basdijkstra/rest-assured-net/issues/26))

### [1.1.0] - 2022-11-12 [YANKED]

### [1.0.0] - 2022-11-03

#### Added

- First public, non-alpha/-beta version of RestAssured.Net 