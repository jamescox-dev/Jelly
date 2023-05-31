namespace Jelly.Tests;

public class TestCommand : ICommand
{
    public int Invokations { get; set; } = 0;
    public Value ReturnValue { get; set; } = "42".ToValue();
    public IEnvironment? EnvironmentPassedToInvoke { get; private set; }
    public IScope? ScopePassedToInvoke { get; private set; }
    public ListValue? ArgsPassedToInvoke { get; private set; }

    public Value Invoke(IEnvironment env, ListValue args)
    {
        EnvironmentPassedToInvoke = env;
        ScopePassedToInvoke = env.CurrentScope;
        ArgsPassedToInvoke = args;
        ++Invokations;
        return ReturnValue;
    }
}