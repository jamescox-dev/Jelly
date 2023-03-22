namespace Jelly.Tests;

using Jelly.Commands;
using Jelly.Values;

public class CounterCommand : ICommand
{
    public int CallCount { get; private set; }
    public int Count { get; set; }
    public int Increment { get; set; } = 1;

    public bool IsMacro => false;

    public Value Invoke(IScope scope, ListValue args)
    {
        ++CallCount;
        Count += Increment;
        return Count.ToString().ToValue();
    }
}