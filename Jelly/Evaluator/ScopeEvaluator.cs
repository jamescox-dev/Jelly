namespace Jelly.Evaluator;

internal class ScopeEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        env.PushScope();
        var result = env.Evaluate(node.GetNode(Keywords.Body));
        env.PopScope();
        return result;
    }
}