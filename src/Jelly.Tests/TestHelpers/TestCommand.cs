namespace Jelly.Tests.TestHelpers;

public class TestCommand : ICommand
{
    public int Invocations { get; set; } = 0;
    public Value ReturnValue { get; set; } = "42".ToValue();
    public IEnv? EnvironmentPassedToInvoke { get; private set; }
    public IScope? ScopePassedToInvoke { get; private set; }
    public ListValue? ArgsPassedToInvoke { get; private set; }

    public Value Invoke(IEnv env, ListValue args)
    {
        EnvironmentPassedToInvoke = env;
        ScopePassedToInvoke = env.CurrentScope;
        ArgsPassedToInvoke = args;
        ++Invocations;
        return ReturnValue;
    }
}