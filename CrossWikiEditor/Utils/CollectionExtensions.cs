using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrossWikiEditor.Utils;

public static class CollectionExtensions
{
    private static Random _random = new Random();
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
    {
        return new ObservableCollection<T>(enumerable);
    }
    
    public static List<T> RandomSubset<T>(this List<T> sourceList, int count)
    {
        if (sourceList == null)
        {
            throw new ArgumentNullException(nameof(sourceList));
        }

        if (count < 0 || count > sourceList.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count should be between 0 and the size of the source list.");
        }

        var subset = new List<T>(count);

        for (int i = 0; i < count; i++)
        {
            int index = _random.Next(sourceList.Count);
            subset.Add(sourceList[index]);
        }

        return subset;
    }
}