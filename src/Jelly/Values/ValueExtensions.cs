namespace Jelly.Values;

public static class ValueExtensions
{
    public static StringValue ToValue(this string str) =>
        new(str);

    public static BooleanValue ToValue(this bool b) => b ? BooleanValue.True : BooleanValue.False;

    public static NumberValue ToValue(this double dbl) =>
        new(dbl);

    public static NumberValue ToValue(this int i) =>
        new(i);

    public static ListValue ToValue(this IEnumerable<Value> list) =>
        new(list);

    public static DictionaryValue ToValue(this IEnumerable<KeyValuePair<Value, Value>> items) =>
        new(items);

    public static DictionaryValue ToDictionaryValue(this IEnumerable<Value> items) =>
        new(items);

    public static int ToIndexOf(this Value indexValue, ListValue ofList)
    {
        var indexDouble = indexValue.ToDouble();

        if (double.IsFinite(indexDouble))
        {
            var index = (int)indexDouble;
            if (index == 0)
            {
                throw new ValueError("index must not be zero.");
            }
            return index > 0 ? index - 1 : ofList.Count + index;
        }

        throw new ValueError("index must be a finite number.");
    }

    public static DictionaryValue ToNode(this Value value) =>
        value.ToDictionaryValue();

    public static DictionaryValue GetNode(this DictionaryValue dict, string key) =>
        dict[key.ToValue()].ToDictionaryValue();

    public static DictionaryValue GetNode(this DictionaryValue dict, Value key) =>
        dict[key].ToDictionaryValue();

    public static DictionaryValue? GetNodeOrNull(this DictionaryValue dict, Value key) =>
        dict.TryGetValue(key, out var value) ? value.ToDictionaryValue() : null;

    public static string GetString(this DictionaryValue dict, Value key) =>
        dict[key].ToString();

    public static string? GetStringOrNull(this DictionaryValue dict, Value key) =>
        dict.TryGetValue(key, out var value) ? value.ToString() : null;

    public static ListValue GetList(this DictionaryValue dict, Value key) =>
        dict[key].ToListValue();
}