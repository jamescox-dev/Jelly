namespace Jelly.Commands.ArgParsers;

public class OrPattern : IArgPattern
{
    readonly IArgPattern[] _subPatterns;

    public OrPattern(params IArgPattern[] subPatterns)
    {
        if (subPatterns.Any())
        {
            _subPatterns = subPatterns;
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    public ArgPatternResult Match(int position, ListValue args)
    {
        ArgPatternResult bestMatchFailure = new ArgPatternResultUnexpected(position);

        foreach (var subPattern in _subPatterns)
        {
            var result = subPattern.Match(position, args);
            if (result is ArgPatternSuccess success)
            {
                return success;
            }
            else if (result.Position >= bestMatchFailure.Position)
            {
                if (result is ArgPatternResultMissing missing && bestMatchFailure is ArgPatternResultMissing bestMissing && bestMatchFailure.Position == missing.Position)
                {
                    bestMatchFailure = new ArgPatternResultMissing(missing.Position, bestMissing.MissingArgs.Union(missing.MissingArgs).ToHashSet());
                }
                else
                {
                    bestMatchFailure = result;
                }
            }
        }

        return bestMatchFailure;
    }
}