namespace Jelly.Interpreter;

public class Interpreter : IInterpreter
{
    static readonly StringValue TypeKey = new StringValue("type");

    Dictionary<string, IInterpreter> _interpreters = new();

    public void AddInterpreter(string nodeType, IInterpreter interpreter)
    {
        _interpreters.Add(nodeType, interpreter);
    }

    public Value Evaluate(DictionaryValue node) =>
        Evaluate(node, this);

    public Value Evaluate(DictionaryValue node, IInterpreter interpreter) =>
        _interpreters[node[TypeKey].ToString()].Evaluate(node, interpreter);
}