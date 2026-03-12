using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Config.Realtime.Abstract;

namespace Soenneker.Config.Realtime.Registrars;

/// <summary>
/// A configuration provider allowing for realtime modification
/// </summary>
public static class RealtimeConfigurationProviderRegistrar
{
    /// <summary>
    /// Adds <see cref="RealtimeConfigurationSource"/> to the <see cref="IConfigurationBuilder"/>. <para/>
    /// </summary>
    public static RealtimeConfigurationProvider AddRealtimeConfiguration(this IConfigurationBuilder builder)
    {
        var provider = new RealtimeConfigurationProvider();

        var source = new RealtimeConfigurationSource(provider);

        builder.Add(source);

        return provider;
    }

    /// <summary>
    /// Adds <see cref="IRealtimeConfigurationProvider"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddRealtimeConfigurationProviderAsSingleton(this IServiceCollection services, RealtimeConfigurationProvider provider)
    {
        services.TryAddSingleton<IRealtimeConfigurationProvider>(provider);

        return services;
    }

    /// <summary>
    /// Adds <see cref="RealtimeConfigurationProvider"/> to the <see cref="IConfigurationBuilder"/>
    /// and registers it as a singleton service in the <see cref="IServiceCollection"/>.
    /// </summary>
    public static IServiceCollection AddRealtimeConfiguration(this IServiceCollection services, IConfigurationBuilder builder)
    {
        // Create the provider
        var provider = new RealtimeConfigurationProvider();

        // Add the provider to the configuration pipeline
        var source = new RealtimeConfigurationSource(provider);
        builder.Add(source);

        // Register the provider in DI
        services.TryAddSingleton<IRealtimeConfigurationProvider>(provider);

        return services;
    }
}