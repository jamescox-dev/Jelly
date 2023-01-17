namespace Jelly.Commands;

using Jelly.Values;

public interface ICommand
{
    public Value Invoke(Scope scope, ListValue args);
}