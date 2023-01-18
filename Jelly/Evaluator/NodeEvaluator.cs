namespace Jelly.Evaluator;

using Jelly.Errors;
using Jelly.Values;

internal class NodeEvaluator : IEvaluator
{
    static readonly StringValue TypeKey = new StringValue("type");

    Dictionary<string, IEvaluator> _evaluators = new();

    public void AddEvaluator(string nodeType, IEvaluator evaluator)
    {
        _evaluators.Add(nodeType, evaluator);
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator)
    {
        if (node.TryGetValue(TypeKey, out var type))
        {
            if (_evaluators.TryGetValue(type.ToString(), out var typeEvaluator))
            {
                return typeEvaluator.Evaluate(scope, node, evaluator);
            }
            else
            {
                throw Error.Eval($"Can not evaluate node of type: '{type}'.");
            }
        }
        else
        {
            throw Error.Eval("Can not evaluate node, not type specified.");
        }
    }
}