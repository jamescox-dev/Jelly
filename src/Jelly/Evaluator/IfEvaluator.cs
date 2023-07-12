namespace Jelly.Evaluator;

internal class IfEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictionaryValue node)
    {
        var condition = node.GetNode(Keywords.Condition);
        var thenBody = node.GetNode(Keywords.Then);
        var elseBody = node.GetNodeOrNull(Keywords.Else);

        var conditionResult = env.Evaluate(condition);
        if (conditionResult.ToBool())
        {
            return env.Evaluate(thenBody);
        }
        else if (elseBody is not null)
        {
            return env.Evaluate(elseBody);
        }

        return Value.Empty;
    }
}