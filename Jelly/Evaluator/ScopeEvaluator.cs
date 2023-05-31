namespace Jelly.Evaluator;

internal class ScopeEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        return env.RunInNestedScope(() => env.Evaluate(node.GetNode(Keywords.Body)));
    }
}