namespace Jelly;

public static class Int32Extensions
{
    public static string WasWere(this int i)
    {
        return Math.Abs(i) == 1 ? $"{i} was" : $"{i} were";
    }

    public static string Of(this int i, string singularName, string pluralName)
    {
        return Math.Abs(i) == 1 ? $"{i} {singularName}" : $"{i} {pluralName}";
    }

    public static string Of(this int i, string singularName)
    {
        var pluralName = singularName.EndsWith('y')
            ? $"{singularName[0 .. ^1]}ies" : $"{singularName}s";

        return i.Of(singularName, pluralName);
    }
}