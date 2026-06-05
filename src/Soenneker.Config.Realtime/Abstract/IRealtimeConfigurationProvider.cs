using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Soenneker.Config.Realtime.Abstract;

/// <summary>
/// A configuration provider allowing for realtime modification
/// </summary>
public interface IRealtimeConfigurationProvider : IConfigurationProvider
{
    /// <summary>
    /// Attempts to execute get.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>A value indicating whether the operation succeeded.</returns>
    new bool TryGet(string key, out string? value);

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    new void Set(string key, string? value);

    /// <summary>
    /// Executes the remove operation.
    /// </summary>
    /// <param name="key">The key.</param>
    void Remove(string key);

    /// <summary>
    /// Gets child keys.
    /// </summary>
    /// <param name="earlierKeys">The earlier keys.</param>
    /// <param name="parentPath">The parent path.</param>
    /// <returns>The result of the operation.</returns>
    new IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath);
}
