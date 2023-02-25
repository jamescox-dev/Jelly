namespace Jelly.Errors;

public class Error : Exception
{
    static readonly SortedDictionary<string, Func<string, Error>> ErrorConstructors = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "/error/arg/", Arg },
        { "/error/eval/", Eval },
        { "/error/name/", Name },
        { "/error/parse/", Parse },
        { "/error/parse/missing/end_token/", MissingEndToken }, 
    };

    public string Type { get; private set; }

    public static string NormalizeType(string original)
    {
        var slashRunsRemoved = string.Join("/", original.Split('/', StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrEmpty(slashRunsRemoved) ? "/" : $"/{slashRunsRemoved}/";
    }

    internal protected Error(string type, string message) : base(message)
    {
        Type = type;
    }

    public bool IsType(string type) => Type.StartsWith(NormalizeType(type), StringComparison.InvariantCultureIgnoreCase);

    public static Error Create(string type, string message)
    {
        if (ErrorConstructors.TryGetValue(NormalizeType(type), out var constructor))
        {
            return constructor(message);
        }
        return new Error(type, message);
    }

    public static Error Arg(string message) => new ArgError(message);

    public static Error Eval(string message) => new EvalError(message);

    public static Error Name(string message) => new NameError(message);

    public static Error Parse(string message) => new ParseError(message);

    public static Error MissingEndToken(string message) => new MissingEndTokenError(message);
}

public class ArgError : Error
{
    internal ArgError(string message) : base("/error/arg/", message) {}
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

// TODO:  Break and Continue.
// interface class Break : Error
// {
//     public Break() : base("/break", "Unexpected 'break' outside of loop.") {}
// }

// interface class Continue : Error
// {
//     public Break() : base("/continue", "Unexpected 'continue' outside of loop.") {}
// }