using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Soenneker.Config.Realtime.Abstract;

/// <summary>
/// A configuration provider allowing for realtime modification
/// </summary>
public interface IRealtimeConfigurationProvider : IConfigurationProvider
{
    new bool TryGet(string key, out string? value);

    new void Set(string key, string? value);

    void Remove(string key);

    new IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath);
}
