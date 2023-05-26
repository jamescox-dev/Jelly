namespace Jelly.Commands;

[Flags]
public enum EvaluationFlags
{
    None = 0,
    Arguments = 1,
    ReturnValue = 2,
    ArgumentsAndReturnValue = Arguments | ReturnValue,
}