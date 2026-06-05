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
    /// Executes the build operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The result of the operation.</returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return _provider;
    }
}
