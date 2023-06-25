namespace Jelly.Commands.ArgParsers;

public class SingleArgPattern : IArgPattern
{
    readonly string _argName;

    public SingleArgPattern(string argName)
    {
        _argName = argName;
    }

    public ArgPatternResult Parse(int position, ListValue args)
    {
        if (position < args.Count)
        {
            return new ArgPatternSuccess(position + 1, new Dictionary<string, Value>
            {
                { _argName, args[position] }
            });
        }

        return new ArgPatternMissing(position, new HashSet<Arg>{ new Arg(_argName) });
    }
}