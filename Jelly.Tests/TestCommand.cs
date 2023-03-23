namespace Jelly.Tests;

using Jelly.Commands;
using Jelly.Values;

public class TestCommand : ICommand
{
    public EvaluationFlags EvaluationFlags { get; set; } = EvaluationFlags.Arguments;
    public Value ReturnValue { get; set; } = "42".ToValue();
    public IScope? ScopePassedToInvoke { get; private set; }
    public ListValue? ArgsPassedToInvoke { get; private set; }

    public Value Invoke(IScope scope, ListValue args)
    {
        ScopePassedToInvoke = scope;
        ArgsPassedToInvoke = args;
        return ReturnValue;
    }
}