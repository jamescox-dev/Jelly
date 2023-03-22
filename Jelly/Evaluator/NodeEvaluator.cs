namespace Jelly.Evaluator;

using Jelly.Errors;
using Jelly.Values;

internal class NodeEvaluator : IEvaluator
{
    static readonly StringValue TypeKey = new StringValue("type");

    Dictionary<string, IEvaluator> _evaluators = new();

    public void AddEvaluator(string nodeType, IEvaluator rootEvaluator)
    {
        _evaluators.Add(nodeType, rootEvaluator);
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
                throw new EvalError($"Can not evaluate node of type: '{type}'.");
            }
        }
        else
        {
            throw new EvalError("Can not evaluate node, no type specified.");
        }
    }
}