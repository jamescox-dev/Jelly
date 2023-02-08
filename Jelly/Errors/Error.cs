namespace Jelly.Errors;

public abstract class Error : Exception
{
    public string Type { get; private set; }

    protected Error(string type, string message) : base(message)
    {
        Type = type;
    }

    public static Error Parse(string message) => new ParseError(message);

    public static Error Name(string message) => new NameError(message);
}

internal class ArgError : Error
{
    public ArgError(string message) : base("/error/arg", message) {}
}

internal class EvalError : Error
{
    public EvalError(string message) : base("/error/eval", message) {}
}

internal class ParseError : Error
{
    public ParseError(string message) : base("/error/parse", message) {}
}

internal class NameError : Error
{
    public NameError(string message) : base("/error/name", message) {}
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