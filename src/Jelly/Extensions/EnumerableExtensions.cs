namespace Jelly.Extensions;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? e)
    {
        return e ?? Enumerable.Empty<T>();
    }

    public static string JoinAnd(this IEnumerable<object> strings)
    {
        var stringsArray = strings.Select(o => o.ToString()).ToArray();

        if (!stringsArray.Any())
        {
            return string.Empty;
        }
        else if (stringsArray.Length == 1)
        {
            return stringsArray[0] ?? string.Empty;
        }

        var initialItems = string.Join(", ", stringsArray[0 .. ^1]);
        return $"{initialItems}, and {stringsArray[^1]}";
    }


    public static string JoinOr(this IEnumerable<object> strings)
    {
        var stringsArray = strings.Select(o => o.ToString()).ToArray();

        if (!stringsArray.Any())
        {
            return string.Empty;
        }
        else if (stringsArray.Length == 1)
        {
            return stringsArray[0] ?? string.Empty;
        }

        var initialItems = string.Join(", ", stringsArray[0 .. ^1]);
        return $"{initialItems}, or {stringsArray[^1]}";
    }
}