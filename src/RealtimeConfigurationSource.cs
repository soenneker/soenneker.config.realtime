using Soenneker.Config.Realtime.Abstract;
using Microsoft.Extensions.Configuration;

namespace Soenneker.Config.Realtime;

/// <inheritdoc cref="IRealtimeConfigurationProvider"/>
public sealed class RealtimeConfigurationSource : IConfigurationSource
{
    private readonly IRealtimeConfigurationProvider _provider;

    public RealtimeConfigurationSource(IRealtimeConfigurationProvider provider)
    {
        _provider = provider;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return _provider;
    }
}
