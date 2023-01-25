namespace Jelly.Commands;

using Jelly.Values;

public class SimpleCommand : ICommand
{
    public delegate Value CommandDelegate(IScope scope, ListValue args);

    public bool IsMacro { get; private set; }

    readonly CommandDelegate _command;

    public SimpleCommand(CommandDelegate command, bool isMacro=false) 
    {
        _command = command;
        IsMacro = isMacro;
    }

    public Value Invoke(IScope scope, ListValue args)
    {
        return _command(scope, args);
    }
}