namespace smooms.app.Utils;

public static class ListExtensions
{
    public static void AddIfNotNullOrEmpty<T>(this List<T> list, T? item)
    {
        if (item?.ToString().IsNullOrWhiteSpace() ?? false)
        {
            list.Add(item);
        }
    }
    
    public static void AddIfNotNull<T>(this List<T> list, T? item)
    {
        if (item != null)
            list.Add(item);
    }
}