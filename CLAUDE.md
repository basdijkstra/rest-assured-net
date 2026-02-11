# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

REST Assured.NET is a C# library providing a fluent API for HTTP API testing, inspired by Java's REST Assured. The core pattern is `Given() -> When() -> Then()` with method chaining.

## Build & Test Commands

```bash
# Build (specify target framework: net6.0, net7.0, net8.0, net9.0, net10.0)
dotnet build --framework net8.0

# Run all tests for a framework
dotnet test RestAssured.Net.Tests --framework net8.0

# Run a specific test by name
dotnet test RestAssured.Net.Tests --framework net8.0 --filter "TestName"

# Run a specific test class
dotnet test RestAssured.Net.Tests --framework net8.0 --filter "FullyQualifiedName~ClassName"

# Mutation testing (Stryker)
dotnet stryker
```

CI runs the full matrix of all 5 frameworks on ubuntu-latest.

## Architecture

**Entry point:** `Dsl.cs` — static `Given()` returns an `ExecutableRequest`, plus a `RestAssuredConfig` property for global configuration.

**Request pipeline:**
1. `ExecutableRequest` — fluent builder for request configuration (headers, body, auth, cookies, query/path params, multipart, GraphQL). HTTP methods (`Get()`, `Post()`, etc.) trigger execution.
2. `RequestSpecification` — reusable configuration holder applied via `.Spec()`. Built with `RequestSpecBuilder`.
3. `HttpRequestProcessor` — internal class handling HttpClient creation, cookie containers, SSL, NTLM auth, and timeout management.

**Response pipeline:**
1. `VerifiableResponse` — verification methods: `StatusCode()`, `Header()`, `ContentType()`, `Cookie()`, `Body()`, `ResponseTime()`, schema validation (`MatchesJsonSchema()`, `MatchesXsd()`, `MatchesInlineDtd()`). Uses NHamcrest matchers for assertions.
2. `ExtractableResponse` — accessed via `.Extract()`. Provides `BodyAsString()`, `Body(path)` (JSONPath/XPath), `DeserializeTo<T>()`, cookie/header extraction.

**Content-type aware processing:** JSON (Newtonsoft.Json + JSONPath), XML (XPath + XmlDocument), HTML (XPath + HtmlAgilityPack). Auto-detected from response headers or overridden with `VerifyAs`/`ExtractAs`.

**Path parameter templating:** Uses Mustache via Stubble.Core with `[paramName]` syntax (custom delimiters).

**Logging:** `RequestLogger`/`ResponseLogger` with configurable log levels and sensitive header/cookie masking via `LogConfiguration`.

**Exceptions:** `ResponseVerificationException`, `ExtractionException`, `DeserializationException`, `RequestCreationException`, `HttpRequestProcessorException`.

## Testing

- **NUnit** with **WireMock.Net** (mock server on port 9876)
- `TestBase` class provides WireMock server setup/teardown and helper methods for test data
- Test models in `RestAssured.Net.Tests/Models/`, schemas in `RestAssured.Net.Tests/Schemas/`
- **Faker.Net** for test data generation
- `RestAssured.Net.DemoService` is an ASP.NET Core app used for integration tests

## Code Style

- StyleCop.Analyzers enforced via `Directory.Build.props` and `StyleCop.ruleset`
- XML documentation required on public members (SA1600)
- File headers required (SA1633)
- `this.` prefix required (SA1101)
- Implicit usings disabled — all usings must be explicit
- Nullable reference types enabled
