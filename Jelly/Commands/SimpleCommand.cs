namespace Jelly.Commands;

public class SimpleCommand : ICommand
{
    public delegate Value CommandDelegate(IScope scope, ListValue args);

    public EvaluationFlags EvaluationFlags => EvaluationFlags.Arguments;

    readonly CommandDelegate _command;

    public SimpleCommand(CommandDelegate command)
    {
        _command = command;
    }

    public Value Invoke(IScope scope, ListValue args)
    {
        return _command(scope, args);
    }
}