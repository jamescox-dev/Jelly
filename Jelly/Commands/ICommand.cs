namespace Jelly.Commands;

using Jelly.Values;

public interface ICommand
{
    EvaluationFlags EvaluationFlags { get; }

    Value Invoke(IScope scope, ListValue args);
}