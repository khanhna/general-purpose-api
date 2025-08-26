namespace GeneralPurpose.Infrastructure.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> query, bool condition,
        Func<TSource, bool> whereExpression) =>
        condition ? query.Where(whereExpression) : query;

    public static IEnumerable<TSource> OrderByDescendingIf<TSource, TKey>(
        this IEnumerable<TSource> source,
        bool condition,
        Func<TSource, TKey> keySelector) =>
        condition ? source.OrderByDescending(keySelector) : source;

    public static IEnumerable<TSource> OrderByAscIf<TSource, TKey>(
        this IEnumerable<TSource> source,
        bool condition,
        Func<TSource, TKey> keySelector) =>
        condition ? source.OrderBy(keySelector) : source;

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    public static bool TryGetElement<T>(this T[] array, int index, out T element)
    {
        if (index < array.Length)
        {
            element = array[index];
            return true;
        }
        element = default(T);
        return false;
    }

    public static bool TryGetElement<T>(this List<T> array, int index, out T element)
    {
        if (index < array.Count)
        {
            element = array[index];
            return true;
        }
        element = default(T);
        return false;
    }

    public static List<T> AddIf<T>(this List<T> list, bool condition, T item)
    {
        if (condition)
        {
            list.Add(item);
        }

        return list;
    }
}