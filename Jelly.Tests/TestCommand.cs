namespace Jelly.Tests;

using Jelly.Commands;
using Jelly.Values;

public class TestCommand : ICommand
{
    public bool IsMacro { get; set; }
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