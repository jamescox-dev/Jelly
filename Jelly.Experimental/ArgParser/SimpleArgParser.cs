namespace Jelly.Experimental;

using System.Collections.Generic;
using System.Collections.Immutable;
using Jelly.Commands.ArgParsers;
using Jelly.Values;

public class SimpleArgParser : IPatternArgParser
{
    public int AcceptsMinArgs { get; }
    public int AcceptsMaxArgs { get; }

    readonly Arg _arg;

    public SimpleArgParser(Arg arg)
    {
        if (arg is OptArg)
        {
            AcceptsMinArgs = 0;
            AcceptsMaxArgs = 1;
        }
        else if (arg is RestArg)
        {
            AcceptsMinArgs = 0;
            AcceptsMaxArgs = int.MaxValue;
        }
        else
        {
            AcceptsMinArgs = 1;
            AcceptsMaxArgs = 1;
        }
        _arg = arg;
    }

    public ParseResult Parse(IReadOnlyList<Value> args)
    {
        if (args.Count < AcceptsMinArgs)
        {
            return new ParseMissingArg(_arg);
        }
        if (args.Count > AcceptsMaxArgs)
        {
            return new ParseUnexpectedArg();
        }

        var argName = _arg.Name.ToValue();
        var result = new Dictionary<string, Value>();
        if (AcceptsMaxArgs > 1)
        {
            result.Add(argName, args.ToValue());
        }
        else
        {
            result.Add(argName, args[0]);
        }
        return new ParseSuccess(AcceptsMaxArgs, result);
    }
}