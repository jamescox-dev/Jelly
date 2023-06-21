namespace Jelly.Experimental;

using System.Collections.Immutable;
using Jelly.Commands.ArgParsers;

public interface IPatternArgParser
{
    ParseResult Parse(IEnumerable<Value> args);
}

public record ParseResult(bool IsSuccessful) {}

public record ParseSuccess(int ConsumedArgs, Dictionary<string, Value> ArgValues) : ParseResult(true) {}

public record ParseMissingArg(string ExpectedArgName) : ParseResult(false) {}

public record ParseUnexpectedArg() : ParseResult(false) {}

public class ArgParser : IPatternArgParser
{
    readonly string _name;

    public ArgParser(string name)
    {
        _name = name;
    }

    public ParseResult Parse(IEnumerable<Value> args)
    {
        var arg = args.FirstOrDefault();
        if (arg is not null)
        {
            return new ParseSuccess(1, new() { { _name, arg } });
        }
        return new ParseMissingArg(_name);
    }
}

public class SeqArgParser : IPatternArgParser
{
    readonly IPatternArgParser[] _subParsers;

    public SeqArgParser(params IPatternArgParser[] subParsers)
    {
        _subParsers = subParsers;
    }

    public ParseResult Parse(IEnumerable<Value> args)
    {
        var argEnum = args.GetEnumerator();
        var argValues = new Dictionary<string, Value>();
        foreach (var subParser in _subParsers)
        {
            var subResult = subParser.Parse(args.Skip(i));
            if (subResult is ParseSuccess subSuccess)
            {
                i += subSuccess.ConsumedArgs;
                foreach (var kvp in subSuccess.ArgValues)
                {
                    argValues.Add(kvp.Key, kvp.Value);
                }
            }
            else
            {
                return subResult;
            }
        }
        return new ParseSuccess(i, argValues);
    }
}