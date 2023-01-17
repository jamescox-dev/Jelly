namespace Jelly.Serializers;

public static class StringSerializer
{
    public static string Serialize(string str) =>
        str.Length == 0 ? "''" : str;
}