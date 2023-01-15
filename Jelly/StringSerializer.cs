namespace Jelly;

public static class StringSerializer
{
    public static object Serialize(string str) =>
        str.Length == 0 ? "''" : str;
}