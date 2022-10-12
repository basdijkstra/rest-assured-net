# RestAssured.NET
This is a C# .NET version of the original [REST Assured](https://rest-assured.io/).

This library is currently in a very initial stage. Expect more features to be added on a very regular basis.

## Examples (updated regularly until proper documentation is in place)

### Checking response status codes
```csharp
[Test]
public void StatusCodeIndicatingSuccessCanBeVerifiedAsInteger()
{
    Given()
    .When()
    .Get("http://localhost:9876/http-status-code-ok")
    .Then()
    .StatusCode(200);
}
```

```csharp
[Test]
public void StatusCodeIndicatingSuccessCanBeVerifiedAsHttpStatusCode()
{
    Given()
    .When()
    .Get("http://localhost:9876/http-status-code-ok")
    .Then()
    .StatusCode(HttpStatusCode.OK);
}
```

```csharp
[Test]
public void StatusCodeIndicatingSuccessCanBeVerifiedUsingNHamcrestEqualToMatcher()
{
    Given()
    .When()
    .Get("http://localhost:9876/http-status-code-ok")
    .Then()
    .StatusCode(NHamcrest.Is.EqualTo(200));
}
```

### Checking response headers
```csharp
[Test]
public void MultipleResponseHeadersCanBeVerified()
{
    Given()
    .When()
    .Get("http://localhost:9876/custom-multiple-response-headers")
    .Then()
    .StatusCode(200)
    .And() // Example of using the And() syntactic sugar method in response verification.
    .Header("custom_header_name", "custom_header_value")
    .And()
    .Header("another_header", "another_value");
}
```

```csharp
[Test]
public void SingleResponseHeaderCanBeVerifiedUsingNHamcrestMatcher()
{
    Given()
    .When()
    .Get("http://localhost:9876/custom-response-header")
    .Then()
    .StatusCode(200)
    .Header("custom_header_name", NHamcrest.Contains.String("tom_header_val"));
}
```

### Checking response Content-Type
```csharp
[Test]
public void ResponseContentTypeHeaderCanBeVerified()
{
    Given()
    .When()
    .Get("http://localhost:9876/custom-response-content-type-header")
    .Then()
    .StatusCode(200)
    .ContentType("application/something");
}
```

```csharp
[Test]
public void ResponseContentTypeHeaderCanBeVerifiedUsingNHamcrestMatcher()
{
    Given()
    .When()
    .Get("http://localhost:9876/custom-response-content-type-header")
    .Then()
    .StatusCode(200)
    .ContentType(NHamcrest.Contains.String("something"));
}
```

### Adding query parameters
Adding a single query parameter:
```csharp
[Test]
public void SingleQueryParameterCanBeSpecified()
{
    Given()
    .QueryParam("name", "john")
    .When()
    .Get("http://localhost:9876/single-query-param")
    .Then()
    .StatusCode(200);
}
```
You can add multiple query parameters by repeated use of `QueryParam()`.

Adding multiple query parameters can also be done using a Dictionary:
```csharp
[Test]
public void MultipleQueryParametersCanBeSpecifiedUsingADictionary()
{
    Dictionary<string, object> queryParams = new Dictionary<string, object>();
    queryParams.Add("name", "john");
    queryParams.Add("id", 12345);

    Given()
    .QueryParams(queryParams)
    .When()
    .Get("http://localhost:9876/multiple-query-params")
    .Then()
    .StatusCode(200);
}
```

### Adding headers (including Content-Type and Accept)
```csharp
[Test]
public void HeaderWithASingleValueCanBeSupplied()
{
    Given()
    .Header("my_header", "my_header_value")
    .When()
    .Get("http://localhost:9876/single-header-value")
    .Then()
    .StatusCode(200);
}
```

```csharp
[Test]
public void ContentTypeAndEncodingCanBeSupplied()
{
    Given()
    .ContentType("application/xml")
    .ContentEncoding(Encoding.ASCII)
    .When()
    .Post("http://localhost:9876/content-type-with-encoding")
    .Then()
    .StatusCode(201);
}
```

```csharp
[Test]
public void AcceptHeaderCanBeSuppliedAsString()
{
    Given()
    .Accept("application/xml")
    .When()
    .Post("http://localhost:9876/accept-header-as-string")
    .Then()
    .StatusCode(201);
}
```

### Adding Authorization details
```csharp
[Test]
public void HeaderWithASingleValueCanBeSupplied()
{
    Given()
    .BasicAuth("username", "password")
    .When()
    .Get("http://localhost:9876/basic-auth")
    .Then()
    .StatusCode(200);
}
```

### Adding a request body (for now only as a string, serialization to come)
```csharp
[Test]
public void PlaintextRequestBodyCanBeSupplied()
{
    Given()
    .Body("Here's a plaintext request body.")
    .When()
    .Post("http://localhost:9876/plaintext-request-body")
    .Then()
    .StatusCode(201);
}
```

```csharp
[Test]
public void JsonStringRequestBodyCanBeSupplied()
{
    Given()
    .Body("{\"id\": 1, \"user\": \"John Doe\"}")
    .When()
    .Post("http://localhost:9876/json-string-request-body")
    .Then()
    .StatusCode(201);
}
```
All usage examples can be found in [the Tests project](https://github.com/basdijkstra/rest-assured-net/tree/main/RestAssuredNet.Tests). These example double as acceptance tests for the library.

First proper NuGet release expected before the end of 2022.