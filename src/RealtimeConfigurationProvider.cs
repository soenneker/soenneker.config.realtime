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
    // Configuration keys are typically case-insensitive.
    private static readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;

    // ":" currently, but treat as a single char for faster IndexOf and checks.
    private const char _delimiter = ':';

    private readonly ConcurrentDictionary<string, string?> _data = new(_comparer);

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
        // Root: merge earlierKeys + all keys, then sort.
        if (parentPath.IsNullOrEmpty())
        {
            int capacity = 0;

            if (earlierKeys is ICollection<string> ekCol)
                capacity += ekCol.Count;

            capacity += _data.Count;

            var list = capacity > 0 ? new List<string>(capacity) : new List<string>();

            foreach (string k in earlierKeys)
                list.Add(k);

            foreach (string k in _data.Keys)
                list.Add(k);

            list.Sort(_comparer);
            return list;
        }

        // Non-root: find immediate children under parentPath WITHOUT allocating "parentPath + ':'".
        int parentLen = parentPath!.Length;

        // Deduplicate child segments.
        HashSet<string>? children = null;

        foreach (string fullKey in _data.Keys)
        {
            // Must be at least "parent:" + 1 char.
            if (fullKey.Length <= parentLen)
                continue;

            // Starts with parentPath (ignore-case) and next char is delimiter.
            if (!fullKey.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase))
                continue;

            if (fullKey[parentLen] != _delimiter)
                continue;

            int segmentStart = parentLen + 1;

            // child is the segment right after "parent:"
            if (segmentStart >= fullKey.Length)
                continue;

            int nextDelimiter = fullKey.IndexOf(_delimiter, segmentStart);
            string child = nextDelimiter < 0 ? fullKey.Substring(segmentStart) : fullKey.Substring(segmentStart, nextDelimiter - segmentStart);

            if (child.Length == 0)
                continue;

            children ??= new HashSet<string>(_comparer);
            children.Add(child);
        }

        // If no matches, we still need to return earlierKeys sorted (per ConfigurationProvider contract expectations).
        int resultCapacity = 0;
        if (earlierKeys is ICollection<string> ek)
            resultCapacity += ek.Count;
        if (children is { Count: > 0 })
            resultCapacity += children.Count;

        var result = resultCapacity > 0 ? new List<string>(resultCapacity) : new List<string>();

        foreach (string k in earlierKeys)
            result.Add(k);

        if (children is not null)
        {
            foreach (string child in children)
                result.Add(child);
        }

        result.Sort(_comparer);
        return result;
    }
}