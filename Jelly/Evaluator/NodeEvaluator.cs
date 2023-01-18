namespace Jelly.Evaluator;

using Jelly.Values;

internal class NodeEvaluator : IEvaluator
{
    static readonly StringValue TypeKey = new StringValue("type");

    Dictionary<string, IEvaluator> _evaluators = new();

    public void AddEvaluator(string nodeType, IEvaluator evaluator)
    {
        _evaluators.Add(nodeType, evaluator);
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator) =>
        _evaluators[node[TypeKey].ToString()].Evaluate(scope, node, evaluator);
}