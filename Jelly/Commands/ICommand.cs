namespace Jelly.Commands;

public interface ICommand
{
    EvaluationFlags EvaluationFlags { get; }

    Value Invoke(IScope scope, ListValue args);
}