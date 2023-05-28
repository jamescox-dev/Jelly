namespace Jelly.Commands;

public abstract class CommandBase : ICommand
{
    public abstract Value Invoke(IEnvironment env, ListValue args);

    protected static ListValue EvaluateArgs(IEnvironment env, ListValue args)
    {
        return args.Select(arg => env.Evaluate(arg.ToNode())).ToValue();
    }
}