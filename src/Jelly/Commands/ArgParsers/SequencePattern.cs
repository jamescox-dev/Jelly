namespace Jelly.Commands.ArgParsers;

public class SequenceArgPattern : IArgPattern
{
    readonly IArgPattern[] _subPatterns;

    public SequenceArgPattern(params IArgPattern[] subPatterns)
    {
        if (!subPatterns.Any())
        {
            throw new ArgumentOutOfRangeException();
        }

        _subPatterns = subPatterns;
    }

    public ArgPatternResult Match(int position, ListValue args)
    {
        var argValues = new Dictionary<string, Value>();

        foreach (var subPattern in _subPatterns)
        {
            var result = subPattern.Match(position, args);
            if (result is ArgPatternSuccess success)
            {
                position = success.Position;
                argValues.AddRange(success.ArgValues);
            }
            else
            {
                return result;
            }
        }

        return new ArgPatternSuccess(position, argValues);
    }
}