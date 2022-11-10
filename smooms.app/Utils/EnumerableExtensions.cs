﻿namespace smooms.app.Utils;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Yield<T>(this T item)
    {
        yield return item;
    }
}