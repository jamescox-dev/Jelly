namespace Jelly.Commands;

using Jelly.Values;

public interface ICommand
{
    bool IsMacro { get; }

    Value Invoke(IScope scope, ListValue args);
}