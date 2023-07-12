namespace Jelly;

public static class DictionaryExtensions
{
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        foreach (var kvp in items)
        {
            dict.Add(kvp);
        }
    }
}