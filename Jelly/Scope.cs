namespace Jelly;

using System;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Values;

public class Scope : IScope
{
    readonly Dictionary<string, Value> _variables = new(StringComparer.InvariantCultureIgnoreCase);
    readonly Dictionary<string, ICommand> _commands = new(StringComparer.InvariantCultureIgnoreCase);
    readonly Dictionary<int, Value> _hiddenValues = new();

    public void DefineVariable(string name, Value initialValue)
    {
        _variables[name] = initialValue;
    }

    public Value GetVariable(string name)
    {
        if (_variables.TryGetValue(name, out var value))
        {
            return value;
        }
        throw new NameError($"Variable '{name}' not defined.");
    }

    public void SetVariable(string name, Value value)
    {
        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
        }
        else
        {
            throw new NameError($"Variable '{name}' not defined.");
        }
    }

    public void DefineCommand(string name, ICommand command)
    {
        _commands[name] = command;
    }

    public ICommand GetCommand(string name)
    {
        if (_commands.TryGetValue(name, out var command))
        {
            return command;
        }
        throw new NameError($"Unknown command '{name}'.");
    }

    public void DefineHiddenValue(int id, Value initialValue)
    {
        _hiddenValues[id] = initialValue;
    }

    public Value GetHiddenValue(int id)
    {
        if (_hiddenValues.TryGetValue(id, out var value))
        {
            return value;
        }
        throw new NameError($"Hidden value:  {id} not defined.");
    }

    public void SetHiddenValue(int id, Value value)
    {
        if (_hiddenValues.ContainsKey(id))
        {
            _hiddenValues[id] = value;
        }
        else
        {
            throw new NameError($"Hidden value:  {id} not defined.");
        }
    }
}