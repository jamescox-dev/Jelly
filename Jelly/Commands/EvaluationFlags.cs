namespace Jelly.Commands;

[Flags]
public enum EvaluationFlags
{
    None = 0,
    Arguments = 1,
    RetrunValue = 2,
    ArgumentsAndReturnValue = Arguments | RetrunValue,
}