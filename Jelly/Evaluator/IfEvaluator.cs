namespace Jelly.Evaluator;

internal class IfEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var condition = node.GetNode(Keywords.Condition);
        var thenBody = node.GetNode(Keywords.Then);

        var conditionResult = env.Evaluate(condition);
        if (conditionResult.ToBool())
        {
            return env.Evaluate(thenBody);
        }
        else if (node.TryGetValue(Keywords.Else, out var elseBodyValue))
        {
            var elseBody = elseBodyValue.ToNode();
            return env.Evaluate(elseBody);
        }

        return Value.Empty;
    }
}