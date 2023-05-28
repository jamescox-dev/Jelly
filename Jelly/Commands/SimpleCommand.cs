namespace Jelly.Commands;

public class SimpleCommand : CommandBase
{
    public delegate Value CommandDelegate(IEnvironment env, ListValue args);

    readonly CommandDelegate _command;

    public SimpleCommand(CommandDelegate command)
    {
        _command = command;
    }

    public override Value Invoke(IEnvironment env, ListValue args)
    {
        ListValue evaluatedArgs = EvaluateArgs(env, args);
        return _command(env, evaluatedArgs);
    }
}