namespace Jelly.Runtime;

public class Scope : IScope
{
    static int _nextId = 0;

    readonly Dictionary<string, Value> _variables = new(StringComparer.InvariantCultureIgnoreCase);
    readonly Dictionary<string, ICommand> _commands = new(StringComparer.InvariantCultureIgnoreCase);
    readonly Dictionary<int, Value> _hiddenValues = new();

    public IScope? OuterScope { get; private set; }

    public static int GenerateId()
    {
        return _nextId++;
    }

    public Scope(IScope? outer=null)
    {
        OuterScope = outer;
    }

    public IEnumerable<string> GetVariableNames(bool localOnly = false)
    {
        if (localOnly || OuterScope is null)
        {
            return _variables.Keys.Where(VariableNameIsVisible);
        }
        else
        {
            return _variables.Keys.Concat(OuterScope.GetVariableNames()).Distinct();
        }
    }

    static bool VariableNameIsVisible(string name)
    {
        return !name.StartsWith("$$");
    }

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
        else if (OuterScope is not null)
        {
            return OuterScope.GetVariable(name);
        }
        throw new NameError($"Variable '{name}' not defined.");
    }

    public Value GetVariable(string name, ValueIndexer indexer)
    {
        return indexer.GetItemOf(GetVariable(name));
    }

    public void SetVariable(string name, Value value)
    {
        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
        }
        else if (OuterScope is not null)
        {
            OuterScope.SetVariable(name, value);
        }
        else
        {
            throw new NameError($"Variable '{name}' not defined.");
        }
    }

    public void SetVariable(string name, ValueIndexer indexer, Value value)
    {
        SetVariable(name, indexer.SetItemOf(GetVariable(name), value));
    }

    public IEnumerable<string> GetCommandNames(bool localOnly = false)
    {
        if (localOnly || OuterScope is null)
        {
            return _commands.Keys;
        }
        else
        {
            return _commands.Keys.Concat(OuterScope.GetCommandNames()).Distinct();
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
        else if (OuterScope is not null)
        {
            return OuterScope.GetCommand(name);
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
        else if (OuterScope is not null)
        {
            return OuterScope.GetHiddenValue(id);
        }
        throw new NameError($"Hidden value:  {id} not defined.");
    }

    public void SetHiddenValue(int id, Value value)
    {
        if (_hiddenValues.ContainsKey(id))
        {
            _hiddenValues[id] = value;
        }
        else if (OuterScope is not null)
        {
            OuterScope.SetHiddenValue(id, value);
        }
        else
        {
            throw new NameError($"Hidden value:  {id} not defined.");
        }
    }
}