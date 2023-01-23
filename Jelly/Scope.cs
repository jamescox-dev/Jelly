namespace Jelly;

using System;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Values;

public class Scope : IScope
{
    readonly Dictionary<string, Value> _variables = new();
    readonly Dictionary<string, ICommand> _commands = new();

    public void DefineVariable(string name, Value initialValue)
    {
        _variables.Add(name, initialValue);
    }

    public Value GetVariable(string name)
    {
        return _variables[name];
    }

    public void SetVariable(string name, Value value)
    {
        _variables[name] = value;
    }

    public void DefineCommand(string name, ICommand command)
    {
        _commands.Add(name, command);
    }

    public ICommand GetCommand(string name)
    {
        if (_commands.TryGetValue(name, out var command))
        {
            return command;
        }
        throw new NameError($"Unknown command '{name}'.");
    }
}