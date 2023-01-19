namespace Jelly.Errors;

public abstract class Error : Exception
{
    public string Type { get; private set; }

    protected Error(string type, string message) : base(message)
    {
        Type = type;
    }
}

internal class EvalError : Error
{
    public EvalError(string message) : base("/error/eval", message) {}
}

internal class ParseError : Error
{
    public ParseError(string message) : base("/error/parse", message) {}
}