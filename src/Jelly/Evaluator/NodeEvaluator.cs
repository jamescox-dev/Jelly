namespace Jelly.Evaluator;

internal class NodeEvaluator : IEvaluator
{
    readonly Dictionary<string, IEvaluator> _evaluators = new();

    public void AddEvaluator(string nodeType, IEvaluator evaluator)
    {
        _evaluators.Add(nodeType, evaluator);
    }

    public Value Evaluate(IEnv env, DictValue node)
    {
        var type = node.GetStringOrNull(Keywords.Type);
        var typeEvaluator = GetEvaluatorForType(type);
        return typeEvaluator.Evaluate(env, node);
    }


    IEvaluator GetEvaluatorForType(string? type)
    {
        if (type is null)
        {
            throw new EvalError("Can not evaluate node, no type specified.");
        }

        if (!_evaluators.TryGetValue(type, out var typeEvaluator))
        {
            throw new EvalError($"Can not evaluate node of type: '{type}'.");
        }

        return typeEvaluator;
    }
}