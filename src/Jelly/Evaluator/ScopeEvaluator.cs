namespace Jelly.Evaluator;

internal class ScopeEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        return env.RunInNestedScope(() => env.Evaluate(node.GetNode(Keywords.Body)));
    }
}