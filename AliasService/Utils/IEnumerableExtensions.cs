namespace AliasService.Utils;

public static class IEnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action) where T : class
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        foreach (T element in items)
        {
            action(element);
        }
    }
}