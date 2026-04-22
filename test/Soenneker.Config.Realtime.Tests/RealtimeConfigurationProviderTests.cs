using AwesomeAssertions;
using Microsoft.Extensions.Configuration;
using Soenneker.Config.Realtime.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Config.Realtime.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class RealtimeConfigurationProviderTests : HostedUnitTest
{
    private readonly IRealtimeConfigurationProvider _provider;
    private readonly IConfiguration _config;

    public RealtimeConfigurationProviderTests(Host host) : base(host)
    {
        _provider = Resolve<IRealtimeConfigurationProvider>(true);
        _config = Resolve<IConfiguration>();
    }

    [Test]
    public void Set_should_set_on_configuration()
    {
        string? oldValue = _config["key"];

        oldValue.Should().Be("old");
        _provider.Set("key", "new");

        string? newValue = _config["key"];

        newValue.Should().Be("new");
    }
}
