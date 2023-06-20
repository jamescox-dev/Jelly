namespace Jelly.Experimental;

using System.Collections.Immutable;
using Jelly.Commands.ArgParsers;

public interface IPatternArgParser
{
    int AcceptsMinArgs { get; }
    int AcceptsMaxArgs { get; }

    ParseResult Parse(string commandName, IReadOnlyList<Value> args, ImmutableSortedDictionary<Value, Value>.Builder result);
}


public record ParseResult(bool Successful) {}

public record ParseSuccess(int ConsumedArguments, Dictionary<string, Value> argValues) : ParseResult(true) {}

public record ParseMissingArg(Arg ExpectedArg) : ParseResult(false) {}

public record ParseUnexpectedArg() : ParseResult(false) {}