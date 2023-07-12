namespace Jelly.Commands.ArgParsers;

public class OneOrMorePattern : IArgPattern
{
    readonly IArgPattern _subPattern;

    public OneOrMorePattern(IArgPattern subPattern)
    {
        _subPattern = subPattern;
    }

    public ArgPatternResult Match(int position, ListValue args)
    {
        var matches = 0;
        var argValues = new Dictionary<string, Value>();

        while (position < args.Count)
        {
            var result = _subPattern.Match(position, args);
            if (result is ArgPatternSuccess success)
            {
                ++matches;
                foreach (var (name, value) in success.ArgValues)
                {
                    var currentValues = argValues.GetValueOrDefault(name, ListValue.EmptyList).ToListValue();
                    var newValues = currentValues.Append(value);
                    argValues[name] = new ListValue(newValues);
                }
                position = success.Position;
            }
            else
            {
                if (matches == 0)
                {
                    return result;
                }
                break;
            }
        }

        return new ArgPatternSuccess(position, argValues);
    }
}