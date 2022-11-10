using System.Diagnostics.CodeAnalysis;

namespace smooms.app.Utils;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? s) 
        => string.IsNullOrWhiteSpace(s);
    
    public static bool IsNullOrEmpty([NotNullWhen(false)] string? s)
        => string.IsNullOrEmpty(s);
}