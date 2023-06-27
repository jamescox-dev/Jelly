namespace Jelly.Commands.ArgParsers;


public class KeywordArgPattern : IArgPattern
{
    readonly string _keyword;

    public KeywordArgPattern(string keyword)
    {
        _keyword = keyword;
    }

    public ArgPatternResult Parse(int position, ListValue args)
    {
        if (position < args.Count)
        {
            if (Node.IsKeyword(args[position].ToNode(), _keyword))
            {
                return new ArgPatternSuccess(position + 1, new Dictionary<string, Value>());
            }
        }
        return new ArgPatternResultMissing(position, new HashSet<Arg> { new KwArg(_keyword) });
    }
}