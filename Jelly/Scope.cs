namespace Jelly;

using System;
using Jelly.Commands;
using Jelly.Values;

public class Scope
{
    readonly Dictionary<string, Value> _variables = new();
    readonly Dictionary<string, ICommand> _commands = new();
    
    public void DefineVariable(string name, StringValue initialValue)
    {
        _variables.Add(name, initialValue);
    }

    public Value GetVariable(string name)
    {
        return _variables[name];
    }

    public void DefineCommand(string name, ICommand command)
    {
        _commands.Add(name, command);
    }

    public ICommand GetCommand(string name)
    {
        return _commands[name];
    }
}