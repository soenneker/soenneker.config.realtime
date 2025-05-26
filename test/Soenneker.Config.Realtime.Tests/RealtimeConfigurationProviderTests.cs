using AwesomeAssertions;
using Microsoft.Extensions.Configuration;
using Soenneker.Config.Realtime.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Config.Realtime.Tests;

[Collection("Collection")]
public class RealtimeConfigurationProviderTests : FixturedUnitTest
{
    private readonly IRealtimeConfigurationProvider _provider;
    private readonly IConfiguration _config;

    public RealtimeConfigurationProviderTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _provider = Resolve<IRealtimeConfigurationProvider>(true);
        _config = Resolve<IConfiguration>();
    }

    [Fact]
    public void Set_should_set_on_configuration()
    {
        string? oldValue = _config["key"];

        oldValue.Should().Be("old");
        _provider.Set("key", "new");

        string? newValue = _config["key"];

        newValue.Should().Be("new");
    }
}
