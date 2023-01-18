namespace Jelly.Errors;

public class Error : Exception
{
    public string Type { get; private set; }

    public Error(string type, string message) : base(message)
    {
        Type = type;
    }

    public static Error Eval(string message) => new Error("/error/eval", message);
}