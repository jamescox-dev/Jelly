namespace Jelly.Commands.ArgParsers;

public class ExactPattern : IArgPattern
{
    readonly IArgPattern _subPattern;

    public ExactPattern(IArgPattern subPattern)
    {
        _subPattern = subPattern;
    }

    public ArgPatternResult Match(int position, ListValue args)
    {
        var result = _subPattern.Match(position, args);
        if (result is ArgPatternSuccess success && success.Position < args.Count)
        {
            return new ArgPatternResultUnexpected(position);
        }

        return result;
    }
}