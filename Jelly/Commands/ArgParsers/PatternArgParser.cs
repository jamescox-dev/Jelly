namespace Jelly.Commands.ArgParsers;

public class PatternArgParser : IArgParser
{
    readonly IArgPattern _pattern;

    public PatternArgParser(IArgPattern pattern)
    {
        _pattern = pattern;
    }

    public DictionaryValue Parse(string commandName, ListValue args)
    {
        var result = _pattern.Match(0, args);

        return result switch {
            ArgPatternSuccess success => new DictionaryValue(success.ArgValues.Select(kvp => new KeyValuePair<Value, Value>(kvp.Key.ToValue(), kvp.Value))),
            ArgPatternResultMissing missing => throw MissingArgError.FromPossibleArgs(commandName, missing.MissingArgs),
            ArgPatternResultUnexpected unexpected => throw new UnexpectedArgError(commandName, unexpected.LastArg),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}