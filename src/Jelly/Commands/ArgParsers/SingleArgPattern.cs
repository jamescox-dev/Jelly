namespace Jelly.Commands.ArgParsers;


public class SingleArgPattern : IArgPattern
{
    readonly string _argName;

    public SingleArgPattern(string argName)
    {
        _argName = argName;
    }

    public ArgPatternResult Match(int position, ListValue args)
    {
        if (position < args.Count)
        {
            return new ArgPatternSuccess(position + 1, new Dictionary<string, Value>
            {
                { _argName, args[position] }
            });
        }

        return new ArgPatternResultMissing(position, new HashSet<Arg> { new(_argName) });
    }
}