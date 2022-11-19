# RestAssured.NET
[![basdijkstra](https://circleci.com/gh/basdijkstra/rest-assured-net.svg?style=shield)](https://app.circleci.com/pipelines/github/basdijkstra/rest-assured-net) ![Nuget](https://img.shields.io/nuget/v/RestAssured.Net?color=blue) ![Nuget](https://img.shields.io/nuget/dt/RestAssured.Net)

This is a C# .NET version of the original [REST Assured](https://rest-assured.io/).

With RestAssured.Net, writing tests for your HTTP APIs is as simple as

```csharp
using static RestAssured.Client;

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

### Want to contribute?
I'm mostly looking for people who want to give the library a spin and let me know what they think, what issues they found and what they're still missing.

Feel free to submit an issue on this repo if you see any room for improvement.