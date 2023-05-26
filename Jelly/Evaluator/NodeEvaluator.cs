namespace Jelly.Evaluator;

internal class NodeEvaluator : IEvaluator
{
    Dictionary<string, IEvaluator> _evaluators = new();

    public void AddEvaluator(string nodeType, IEvaluator evaluator)
    {
        _evaluators.Add(nodeType, evaluator);
    }

    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        if (node.TryGetValue(Keywords.Type, out var type))
        {
            if (_evaluators.TryGetValue(type.ToString(), out var typeEvaluator))
            {
                return typeEvaluator.Evaluate(env, node);
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