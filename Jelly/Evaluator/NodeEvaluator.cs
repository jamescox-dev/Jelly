namespace Jelly.Evaluator;

internal class NodeEvaluator : IEvaluator
{
    Dictionary<string, IEvaluator> _evaluators = new();

    public void AddEvaluator(string nodeType, IEvaluator evaluator)
    {
        _evaluators.Add(nodeType, evaluator);
    }

    public Value Evaluate(IEnv env, DictionaryValue node)
    {
        var type = node.GetStringOrNull(Keywords.Type);

        if (type is not null)
        {
            if (_evaluators.TryGetValue(type, out var typeEvaluator))
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