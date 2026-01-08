using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Soenneker.Config.Realtime.Abstract;
using Soenneker.Extensions.String;

namespace Soenneker.Config.Realtime;

/// <inheritdoc cref="IRealtimeConfigurationProvider"/>
public sealed class RealtimeConfigurationProvider : ConfigurationProvider, IRealtimeConfigurationProvider
{
    private readonly ConcurrentDictionary<string, string?> _data = new();

    public override bool TryGet(string key, out string? value) => _data.TryGetValue(key, out value);

    public override void Set(string key, string? value)
    {
        // Avoid AddOrUpdate delegate allocations and avoid OnReload() if unchanged.
        while (true)
        {
            if (_data.TryGetValue(key, out string? existing))
            {
                if (string.Equals(existing, value, StringComparison.Ordinal))
                    return; // no change

                if (_data.TryUpdate(key, value, existing))
                {
                    OnReload();
                    return;
                }

                // race: try again
                continue;
            }

            if (_data.TryAdd(key, value))
            {
                OnReload();
                return;
            }

            // race: someone added; loop and TryUpdate
        }
    }

    public void Remove(string key)
    {
        if (_data.TryRemove(key, out _))
            OnReload();
    }

    public override IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        // Config keys are typically case-insensitive. Also faster/more deterministic than CurrentCulture comparisons.
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        // Root: merge earlierKeys + all keys, then sort.
        if (parentPath.IsNullOrEmpty())
        {
            var list = new List<string>();

            if (earlierKeys is ICollection<string> ekCol)
                list.Capacity = ekCol.Count + _data.Count;

            foreach (string k in earlierKeys)
                list.Add(k);

            foreach (string k in _data.Keys)
                list.Add(k);

            list.Sort(comparer);
            return list;
        }

        // Non-root: find immediate children under parentPath.
        string prefix = string.Concat(parentPath, ConfigurationPath.KeyDelimiter);
        int prefixLen = prefix.Length;

        // Deduplicate child segments.
        var children = new HashSet<string>(comparer);

        foreach (string fullKey in _data.Keys)
        {
            if (!fullKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                continue;

            // child is the segment right after "parentPath:"
            if (prefixLen >= fullKey.Length)
                continue;

            int nextDelimiter = fullKey.IndexOf(ConfigurationPath.KeyDelimiter, prefixLen);
            string child = nextDelimiter < 0 ? fullKey.Substring(prefixLen) : fullKey.Substring(prefixLen, nextDelimiter - prefixLen);

            if (child.Length != 0)
                children.Add(child);
        }

        var result = new List<string>();

        if (earlierKeys is ICollection<string> ek)
            result.Capacity = ek.Count + children.Count;

        foreach (string k in earlierKeys)
            result.Add(k);

        foreach (string child in children)
            result.Add(child);

        result.Sort(comparer);
        return result;
    }
}