namespace Jelly.Commands;

public class SimpleCommand : CommandBase
{
    public delegate Value CommandDelegate(ListValue args);

    readonly CommandDelegate _command;

    public SimpleCommand(CommandDelegate command)
    {
        _command = command;
    }

    public override Value Invoke(IEnv env, ListValue unevaluatedArgs)
    {
        ListValue args = EvaluateArgs(env, unevaluatedArgs);
        return _command(args);
    }
}