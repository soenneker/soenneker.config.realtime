using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Soenneker.Config.Realtime.Abstract;

/// <summary>
/// A configuration provider allowing for realtime modification
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
    /// Sets the value.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="value">Configuration value to publish; null removes the key.</param>
    new void Set(string key, string? value);

    /// <summary>
    /// Removes realtime Configuration Provider for the Realtime Configuration Provider.
    /// </summary>
    /// <param name="key">Key used to locate the target entry.</param>
    void Remove(string key);

    /// <summary>
    /// Gets child keys.
    /// </summary>
    /// <param name="earlierKeys">earlier Keys to process.</param>
    /// <param name="parentPath">Path of the parent to use.</param>
    /// <returns>The requested collection.</returns>
    new IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath);
}
