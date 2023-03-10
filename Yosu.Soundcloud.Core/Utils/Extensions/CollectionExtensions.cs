using System.Collections.Generic;

namespace Yosu.Soundcloud.Core.Utils.Extensions;

internal static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        foreach (var i in items)
            source.Add(i);
    }
}