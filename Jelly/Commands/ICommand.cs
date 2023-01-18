namespace Jelly.Commands;

using Jelly.Values;

public interface ICommand
{
    public Value Invoke(IScope scope, ListValue args);
}