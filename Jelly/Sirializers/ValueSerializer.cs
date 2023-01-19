namespace Jelly.Serializers;

using Jelly.Values;

public static class ValueSerializer
{
    public static string Serialize(Value value) =>
        value.ToString().Length == 0 ? "''" : value.ToString();
}