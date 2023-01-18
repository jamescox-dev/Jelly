namespace Jelly.Values;

public static class ValueExtensions
{
    public static StringValue ToValue(this string str) => 
        new StringValue(str);

    public static ListValue ToValue(this IEnumerable<Value> list) => 
        new ListValue(list);

    public static DictionaryValue ToValue(this IEnumerable<KeyValuePair<Value, Value>> items) => 
        new DictionaryValue(items);

    public static DictionaryValue ToDictionaryValue(this IEnumerable<Value> items) => 
        new DictionaryValue(items);
}