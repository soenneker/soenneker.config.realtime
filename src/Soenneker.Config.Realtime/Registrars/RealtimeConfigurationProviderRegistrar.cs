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
    /// <param name="builder">Builder to configure.</param>
    /// <returns>The resulting realtime Configuration Provider.</returns>
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
    /// <param name="services">Service collection that receives the registration.</param>
    /// <param name="provider">Provider for the add realtime configuration provider as singleton operation.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddRealtimeConfigurationProviderAsSingleton(this IServiceCollection services, RealtimeConfigurationProvider provider)
    {
        services.Replace(ServiceDescriptor.Singleton<IRealtimeConfigurationProvider>(provider));

        return services;
    }

    /// <summary>
    /// Adds <see cref="RealtimeConfigurationProvider"/> to the <see cref="IConfigurationBuilder"/>
    /// and registers it as a singleton service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <param name="builder">Builder to configure.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddRealtimeConfiguration(this IServiceCollection services, IConfigurationBuilder builder)
    {
        // Create the provider
        var provider = new RealtimeConfigurationProvider();

        // Add the provider to the configuration pipeline
        var source = new RealtimeConfigurationSource(provider);
        builder.Add(source);

        // Register the provider in DI
        services.Replace(ServiceDescriptor.Singleton<IRealtimeConfigurationProvider>(provider));

        return services;
    }
}
