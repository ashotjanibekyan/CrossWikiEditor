namespace CrossWikiEditor.Core.Utils;

public static class CollectionExtensions
{
    private static readonly Random Random = new();

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
    {
        return new ObservableCollection<T>(enumerable);
    }

    public static List<T> RandomSubset<T>(this List<T>? sourceList, int count)
    {
        ArgumentNullException.ThrowIfNull(sourceList);

        if (count < 0 || count > sourceList.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count should be between 0 and the size of the source list.");
        }

        var subset = new List<T>(count);
        var indexHash = new HashSet<int>();
        while (indexHash.Count < count)
        {
            indexHash.Add(Random.Next(sourceList.Count));
        }

        subset.AddRange(indexHash.Select(index => sourceList[index]));

        return subset;
    }

    public static void Remove<T>(this ObservableCollection<T> obj, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            obj.Remove(item);
        }
    }

    public static void AddRange<T>(this ObservableCollection<T> obj, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            obj.Add(item);
        }
    }

    public static void AddIf<T>(this ICollection<T> obj, T value, bool condition)
    {
        if (condition)
        {
            obj.Add(value);
        }
    }
}