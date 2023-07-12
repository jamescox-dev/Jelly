namespace Jelly.Commands;

public abstract class CommandBase : ICommand
{
    public abstract Value Invoke(IEnv env, ListValue unevaluatedArgs);

    protected static ListValue EvaluateArgs(IEnv env, ListValue unevaluatedArgs)
    {
        return unevaluatedArgs.Select(arg => env.Evaluate(arg.ToNode())).ToValue();
    }
}