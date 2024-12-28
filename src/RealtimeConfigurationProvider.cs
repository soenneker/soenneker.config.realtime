using System;
using Microsoft.Extensions.Configuration;
using Soenneker.Config.Realtime.Abstract;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Soenneker.Extensions.String;

namespace Soenneker.Config.Realtime;

/// <inheritdoc cref="IRealtimeConfigurationProvider"/>
public class RealtimeConfigurationProvider : ConfigurationProvider, IRealtimeConfigurationProvider
{
    private readonly ConcurrentDictionary<string, string?> _data = new();

    public override bool TryGet(string key, out string? value)
    {
        return _data.TryGetValue(key, out value);
    }

    public override void Set(string key, string? value)
    {
        _data.AddOrUpdate(key, value, (_, _) => value);
        OnReload();
    }

    public void Remove(string key)
    {
        if (_data.TryRemove(key, out _))
            OnReload();
    }

    public override IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        // If parentPath is null, treat it as the root and return top-level keys
        if (parentPath.IsNullOrEmpty())
        {
            return earlierKeys.Concat(_data.Keys)
                .OrderBy(k => k);
        }

        // Filter keys that start with the parentPath and extract immediate children
        IEnumerable<string> childKeys = _data.Keys
            .Where(k => k.StartsWith($"{parentPath}:", StringComparison.OrdinalIgnoreCase))
            .Select(k => k.Substring(parentPath.Length + 1)) // Extract the child portion
            .Select(k => k.Split(':')[0]) // Only include the immediate child segment
            .Distinct();

        return earlierKeys.Concat(childKeys).OrderBy(k => k);
    }
}