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

    /// <summary>
    /// Builds realtime Configuration Source for the Realtime Configuration Source.
    /// </summary>
    /// <param name="builder">Builder to configure.</param>
    /// <returns>The resulting configuration Provider.</returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return _provider;
    }
}
