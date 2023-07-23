namespace Jelly.Values;

public static class ValueExtensions
{
    public static StrValue ToValue(this string str) =>
        new(str);

    public static BoolValue ToValue(this bool b) => b ? BoolValue.True : BoolValue.False;

    public static NumValue ToValue(this double dbl) =>
        new(dbl);

    public static NumValue ToValue(this int i) =>
        new(i);

    public static ListValue ToValue(this IEnumerable<Value> list) =>
        new(list);

    public static DictValue ToValue(this IEnumerable<KeyValuePair<Value, Value>> items) =>
        new(items);

    public static DictValue ToDictionaryValue(this IEnumerable<Value> items) =>
        new(items);

    public static int ToIndexOf(this Value indexValue, ListValue ofList) => ToIndexOfLength(indexValue, ofList.Count);

    public static int ToIndexOfLength(this Value indexValue, int length)
    {
        var indexDouble = indexValue.ToDouble();

        if (double.IsFinite(indexDouble))
        {
            var index = (int)indexDouble;
            if (index == 0)
            {
                throw new ValueError("index must not be zero.");
            }
            return index > 0 ? index - 1 : length + index;
        }

        throw new ValueError("index must be a finite number.");
    }

    public static DictValue ToNode(this Value value) =>
        value.ToDictionaryValue();

    public static DictValue GetNode(this DictValue dict, string key) =>
        dict[key.ToValue()].ToDictionaryValue();

    public static DictValue GetNode(this DictValue dict, Value key) =>
        dict[key].ToDictionaryValue();

    public static DictValue? GetNodeOrNull(this DictValue dict, Value key) =>
        dict.TryGetValue(key, out var value) ? value.ToDictionaryValue() : null;

    public static string GetString(this DictValue dict, Value key) =>
        dict[key].ToString();

    public static string? GetStringOrNull(this DictValue dict, Value key) =>
        dict.TryGetValue(key, out var value) ? value.ToString() : null;

    public static ListValue GetList(this DictValue dict, Value key) =>
        dict[key].ToListValue();

    // TODO:  This needs testing, and should be a command line option...
    public static object? ToClr(this Value? value)
    {
        return value switch
        {
            BoolValue boolean => boolean.ToBool(),
            NumValue number => number.ToDouble(),
            StrValue str => str.ToString(),
            ListValue list => list.Select(v => ToClr(v)).ToList(),
            DictValue dict => new Dictionary<string, object?>(
                dict.ToEnumerable().Select(kvp => new KeyValuePair<string, object?>(ToClr(kvp.Key)?.ToString() ?? string.Empty, ToClr(kvp.Value)))),
            _ => null
        };
    }
}