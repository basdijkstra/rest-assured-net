# RestAssured.Net
![github-actions-ci](https://github.com/basdijkstra/rest-assured-net/actions/workflows/ci.yml/badge.svg) ![Nuget](https://img.shields.io/nuget/v/RestAssured.Net?color=blue) ![Nuget](https://img.shields.io/nuget/dt/RestAssured.Net)

RestAssured.Net brings the power of <a href="https://rest-assured.io" target="_blank">REST Assured</a> to the C# / .NET ecosystem.

With RestAssured.Net, writing tests for your HTTP APIs is as simple as

```csharp
using static RestAssured.Dsl;

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

### Where can I get RestAssured.Net?
You can add RestAssured.Net to your project using [NuGet](https://www.nuget.org/packages/RestAssured.Net):

`dotnet add package RestAssured.Net` or `nuget install RestAssured.Net`

RestAssured.Net is also available through [GitHub Packages](https://github.com/basdijkstra/rest-assured-net/pkgs/nuget/RestAssured.Net).

### Want to contribute?
I'm mostly looking for people who want to give the library a spin and let me know what they think, what issues they found and what they're still missing.

Feel free to submit an issue on this repo if you see any room for improvement.
