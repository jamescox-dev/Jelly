namespace Jelly.Commands;

public abstract class CommandBase : ICommand
{
    public abstract Value Invoke(IEnvironment env, ListValue unevaluatedArgs);

    protected static ListValue EvaluateArgs(IEnvironment env, ListValue unevaluatedArgs)
    {
        return unevaluatedArgs.Select(arg => env.Evaluate(arg.ToNode())).ToValue();
    }
}