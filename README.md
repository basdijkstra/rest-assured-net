# RestAssured.NET
[![basdijkstra](https://circleci.com/gh/basdijkstra/rest-assured-net.svg?style=shield)](https://app.circleci.com/pipelines/github/basdijkstra/rest-assured-net) ![Nuget](https://img.shields.io/nuget/v/RestAssured.Net?color=blue) ![Nuget](https://img.shields.io/nuget/dt/RestAssured.Net)

This is a C# .NET version of the original [REST Assured](https://rest-assured.io/).

With RestAssured.Net, writing tests for your HTTP APIs is as simple as

```csharp
[Test]
public void DemonstrateRestAssuredNetEaseOfUse()
{
    Given()
    .When()
    .Get("http://api.zippopotam.us/us/90210")
    .Then()
    .StatusCode(200)
    .And()
    .Body("$.places[0].state", NHamcrest.Is.EqualTo("California"));
}
```

All features of the library are described and demonstrated in the [RestAssured.Net Usage Guide](https://github.com/basdijkstra/rest-assured-net/wiki/Usage-Guide).

This library is currently under heavy development. Expect more features to be added on a very regular basis.

First proper NuGet release expected before the end of 2022.