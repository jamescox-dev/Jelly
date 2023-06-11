namespace Jelly.Errors;

public class Error : Exception
{
    static readonly SortedDictionary<string, Func<string, Value, Error>> ErrorConstructors = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "/break/", Break },
        { "/continue/", Continue },
        { "/error/arg/", Arg },
        { "/error/arg/missing/", MissingArg },
        { "/error/eval/", Eval },
        { "/error/name/", Name },
        { "/error/parse/", Parse },
        { "/error/parse/missing/end_token/", MissingEndToken },
        { "/error/type/", BuildType },
        { "/error/value/", BuildValue },
        { "/return/", Return },
    };

    public string Type { get; private set; }

    public Value Value { get; private set; }

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

    public static Error MissingArg(string message) => new MissingArgError(message);

    public static Error MissingArg(string message, Value _) => MissingArg(message);

    public static Error MissingArg(params Expectation[] expectations) => new MissingArgError(0, expectations);

    public static Error MissingArg(int matchScore, params Expectation[] expectations) =>
        new MissingArgError(matchScore, expectations);

    public static Error Eval(string message) => new EvalError(message);

    public static Error Eval(string message, Value _) => Eval(message);

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

    internal ArgError(string subType, string message) : base(Error.NormalizeType($"/error/arg/{subType}"), message) {}
}

public class MissingArgError : ArgError
{
    public int MatchedArgumentCount { get; }
    public IReadOnlyList<Expectation> Expectations => _expectations;

    readonly Expectation[] _expectations;

    internal MissingArgError(int matchScore, Expectation[] expectations) : base("missing", CreateExpectationMessage(expectations))
    {
        MatchedArgumentCount = matchScore;
        _expectations = expectations;
    }

    internal MissingArgError(string message) : base("missing", message)
    {
        _expectations = Array.Empty<Expectation>();
    }

    static string CreateExpectationMessage(Expectation[] expectations)
    {
        return "";
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

public class Break : Error
{
    public Break() : base("/break/", "Unexpected 'break' outside of loop.") {}
}

public class Continue : Error
{
    public Continue() : base("/continue/", "Unexpected 'continue' outside of loop.") {}
}