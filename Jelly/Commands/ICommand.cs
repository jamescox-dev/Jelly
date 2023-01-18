namespace Jelly.Commands;

using Jelly.Values;

public interface ICommand
{
    public bool IsMacro { get; }

    public Value Invoke(IScope scope, ListValue args);
}