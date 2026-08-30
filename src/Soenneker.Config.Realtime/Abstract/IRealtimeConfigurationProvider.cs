using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Soenneker.Config.Realtime.Abstract;

/// <summary>
/// Provides process-local, thread-safe configuration values that can be changed at runtime.
/// </summary>
public interface IRealtimeConfigurationProvider : IConfigurationProvider
{
    /// <summary>
    /// Attempts to retrieve the entry for the specified key without creating a new value.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="value">Receives the matching value when the lookup succeeds.</param>
    /// <returns>true if a matching value was found and assigned to the output parameter; otherwise, false.</returns>
    new bool TryGet(string key, out string? value);

    /// <summary>
    /// Sets a value and publishes a configuration reload notification when it changes.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="value">Configuration value to publish; null removes the key.</param>
    new void Set(string key, string? value);

    /// <summary>
    /// Removes a value and publishes a configuration reload notification when the key existed.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    void Remove(string key);

    /// <summary>
    /// Returns immediate child key segments beneath a configuration path.
    /// </summary>
    /// <param name="earlierKeys">Child keys supplied by providers earlier in the configuration pipeline.</param>
    /// <param name="parentPath">The parent configuration path, or null for root keys.</param>
    /// <returns>The merged child keys in configuration key order.</returns>
    new IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath);
}
