namespace Jelly.Evaluator;

internal class IfEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        throw new NotImplementedException();
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var conditionResult = rootEvaluator.Evaluate(scope, node[Keywords.Condition].ToDictionaryValue());
        if (conditionResult.ToBool())
        {
            return rootEvaluator.Evaluate(scope, node[Keywords.Then].ToDictionaryValue());
        }
        else if (node.TryGetValue(Keywords.Else, out var elseBody))
        {
            return rootEvaluator.Evaluate(scope, elseBody.ToDictionaryValue());
        }
        return Value.Empty;
    }
}