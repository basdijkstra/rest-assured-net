# RestAssured.NET
This is a C# .NET version of the original [REST Assured](https://rest-assured.io/).

This library is currently in a very initial stage. Expect more features to be added on a very regular basis.

## Examples (will be updated continually until proper documentation is in place)

### Checking response status codes
```csharp
[Test]
public void HttpGetCanBeUsed()
{
    Given()
    .When()
    .Get("http://api.zippopotam.us/us/90210")
    .Then()
    .StatusCode(200);
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

First proper NuGet release expected before the end of 2022.