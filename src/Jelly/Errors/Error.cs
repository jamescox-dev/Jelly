namespace Jelly.Errors;

using Humanizer;

public class Error : Exception
{
    static readonly SortedDictionary<string, Func<string, Value, Error>> ErrorConstructors = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "/break/", Break },
        { "/continue/", Continue },
        { "/error/arg/", Arg },
        { "/error/arg/missing/", MissingArg },
        { "/error/arg/unexpected/", UnexpectedArg },
        { "/error/eval/", Eval },
        { "/error/index/", Index },
        { "/error/io/", Io },
        { "/error/key/", Key },
        { "/error/name/", Name },
        { "/error/parse/", Parse },
        { "/error/parse/missing/end_token/", MissingEndToken },
        { "/error/type/", BuildType },
        { "/error/value/", BuildValue },
        { "/return/", Return },
    };

    public string Type { get; }

    public Value Value { get; }

    public int StartPosition { get; internal set; } = -1;

    public int EndPosition { get; internal set; } = -1;

    public static void RethrowUnhandledClrExceptionsAsJellyErrors(Action action)
    {
        try
        {
            action();
        }
        catch (Error)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Error($"/error/sys/{ex.GetType().Name.Underscore()}/", ex.Message);
        }
    }

    public static string NormalizeType(string original)
    {
        var slashRunsRemoved = string.Join("/", original.Split('/', StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrEmpty(slashRunsRemoved) ? "/" : $"/{slashRunsRemoved}/";
    }

    internal protected Error(string type, string message) : base(message)
    {
        Type = type;
        Value = Value.Empty;
    }

    internal protected Error(string type, string message, Value value) : base(message)
    {
        Type = NormalizeType(type);
        Value = value;
    }

    public bool IsType(string type) => Type.StartsWith(NormalizeType(type), StringComparison.InvariantCultureIgnoreCase);

    public static Error Create(string type, string message)
    {
        return Create(type, message, Value.Empty);
    }

    public static Error Create(string type, string message, Value value)
    {
        if (ErrorConstructors.TryGetValue(NormalizeType(type), out var constructor))
        {
            return constructor(message, value);
        }
        return new Error(type, message, value);
    }

    public static Error Break() => new Break();

    public static Error Break(string _, Value __) => Break();

    public static Error Continue() => new Continue();

    public static Error Continue(string _, Value __) => Continue();

    public static Error Arg(string message) => new ArgError(message);

    public static Error Arg(string message, Value _) => Arg(message);

    public static Error UnexpectedArg(string message) => new UnexpectedArgError(message);

    public static Error UnexpectedArg(string message, Value _) => UnexpectedArg(message);

    public static Error MissingArg(string message) => new MissingArgError(message);

    public static Error MissingArg(string message, Value _) => MissingArg(message);

    public static Error Eval(string message) => new EvalError(message);

    public static Error Eval(string message, Value _) => Eval(message);

    public static Error Index(string message) => new IndexError(message);

    public static Error Index(string message, Value _) => Index(message);

    public static Error Io(string message) => new IoError(message);

    public static Error Io(string message, Value _) => Io(message);

    public static Error Key(string message) => new KeyError(message);

    public static Error Key(string message, Value _) => Key(message);

    public static Error Name(string message) => new NameError(message);

    public static Error Name(string message, Value _) => Name(message);

    public static Error Parse(string message) => new ParseError(message);

    public static Error Parse(string message, Value _) => Parse(message);

    public static Error MissingEndToken(string message) => new MissingEndTokenError(message);

    public static Error MissingEndToken(string message, Value _) => MissingEndToken(message);

    public static Error BuildValue(string message) => new ValueError(message);

    public static Error BuildValue(string message, Value _) => BuildValue(message);

    public static Error BuildType(string message) => new TypeError(message);

    public static Error BuildType(string message, Value _) => BuildType(message);

    public static Error Return(string _) => new Return();

    public static Error Return(string _, Value value) => new Return(value);
}

public class ArgError : Error
{
    internal ArgError(string message) : base("/error/arg/", message) {}

    internal ArgError(string type, string message) : base(type, message) {}
}

public class UnexpectedArgError : ArgError
{
    internal UnexpectedArgError(string commandName, Arg? lastArg)
        : this(BuildStandardUnexpectedMessage(commandName, lastArg)) {}

    internal UnexpectedArgError(string commandName, int expectedArgCount, int actualArgCount)
        : this(BuildStandardMessage(commandName, expectedArgCount, actualArgCount)) {}

    internal UnexpectedArgError(string commandName, int expectedMinArgCount, int expectedMaxArgCount, int actualArgCount)
        : this(BuildStandardRangeMessage(commandName, expectedMinArgCount, expectedMaxArgCount, actualArgCount)) {}

    internal UnexpectedArgError(string message) : base("/error/arg/unexpected/", message) {}

    static string BuildStandardUnexpectedMessage(string commandName, Arg? lastArg)
    {
        return lastArg is null
            ? $"{commandName} received unexpected argument"
            : $"{commandName} received unexpected argument after {lastArg.Name}.";
    }

    static string BuildStandardMessage(string commandName, int expectedArgCount, int actualArgCount)
    {
        return $"{commandName} takes {expectedArgCount.Of("argument")} but {actualArgCount.WasWere()} given.";
    }

    static string BuildStandardRangeMessage(
        string commandName, int expectedMinArgCount, int expectedMaxArgCount, int actualArgCount)
    {
        return $"{commandName} takes from {expectedMinArgCount} to {expectedMaxArgCount.Of("argument")} but {actualArgCount.WasWere()} given.";
    }
}

public class MissingArgError : ArgError
{
    internal MissingArgError(string commandName, IEnumerable<Arg> expectedArg)
        : this(BuildStandardMessage(commandName, expectedArg.ToList())) {}

    internal MissingArgError(string message) : base("/error/arg/missing/", message) {}

    public static MissingArgError FromPossibleArgs(string commandName, IReadOnlySet<Arg> possibleArgs)
    {
        var expectedKeywordsString = possibleArgs.Where(a => a is KwArg).Select(a => a.Name).OrderBy(n => n).JoinOr();
        var expectedArgNamesString = possibleArgs.Where(a => a is not KwArg).Select(a => a.Name).OrderBy(n => n).JoinOr();
        var message = string.IsNullOrEmpty(expectedKeywordsString)
            ? $"{commandName} missing argument, expected:  {expectedArgNamesString}."
            : $"{commandName} missing argument, expected keyword:  {expectedKeywordsString}, or value for:  {expectedArgNamesString}.";

        return new MissingArgError(message);
    }

    static string BuildStandardMessage(string commandName, IList<Arg> expectedArg)
    {
        var argGroups = expectedArg.GroupBy(a => a.GetType());

        var missingArgCountString = expectedArg.Count == 1 ? "1 required argument" : $"{expectedArg.Count} required arguments";
        var expectedArgNamesString = expectedArg.Select(a => a.Name).JoinAnd();
        return $"{commandName} missing {missingArgCountString}:  {expectedArgNamesString}.";
    }
}

public class EvalError : Error
{
    internal EvalError(string message) : base("/error/eval/", message) {}
}

public class ParseError : Error
{
    internal ParseError(string message) : base("/error/parse/", message) {}
    internal ParseError(string type, string message) : base(type, message) {}
}

public class MissingEndTokenError : ParseError
{
    internal MissingEndTokenError(string message) : base("/error/parse/missing/end_token/", message) {}
}

public class NameError : Error
{
    internal NameError(string message) : base("/error/name/", message) {}
}

public class IndexError : Error
{
    internal IndexError(string message) : base("/error/index/", message) {}
}

public class Return : Error
{
    internal Return() : this(Value.Empty) {}

    internal Return(Value value) : base("/return/", "Unexpected 'return' outside of def.", value) {}
}

public class TypeError : Error
{
    internal TypeError(string message) : base("/error/type/", message) {}
}

public class ValueError : Error
{
    internal ValueError(string message) : base("/error/value/", message) {}
}

public class IoError : Error
{
    internal IoError(string message) : base("/error/io/", message) {}
}

public class KeyError : Error
{
    internal KeyError(string message) : base("/error/key/", message) {}
}

public class Break : Error
{
    public Break() : base("/break/", "Unexpected 'break' outside of loop.") {}
}

public class Continue : Error
{
    public Continue() : base("/continue/", "Unexpected 'continue' outside of loop.") {}
}