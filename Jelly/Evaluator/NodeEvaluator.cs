namespace Jelly.Evaluator;

using Jelly.Values;

public class NodeEvaluator : IEvaluator
{
    static readonly StringValue TypeKey = new StringValue("type");

    Dictionary<string, IEvaluator> _interpreters = new();

    public void AddEvaluator(string nodeType, IEvaluator interpreter)
    {
        _interpreters.Add(nodeType, interpreter);
    }

    public Value Evaluate(Scope scope, DictionaryValue node, IEvaluator interpreter) =>
        _interpreters[node[TypeKey].ToString()].Evaluate(scope, node, interpreter);
}