namespace Jelly;

using Jelly.Values;

public class Scope
{
    readonly Dictionary<string, Value> _variables = new();

    public void DefineVariable(string name, StringValue initialValue)
    {
        _variables.Add(name, initialValue);
    }

    public Value GetVariable(string name)
    {
        return _variables[name];
    }
}