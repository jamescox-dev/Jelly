namespace Jelly.Tests;

public class CounterCommand : ICommand
{
    public int CallCount { get; private set; }
    public int Count { get; set; }
    public int Increment { get; set; } = 1;

    public Value Invoke(IEnvironment env, ListValue args)
    {
        ++CallCount;
        Count += Increment;
        return Count.ToString().ToValue();
    }
}