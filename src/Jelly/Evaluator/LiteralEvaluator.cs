namespace Jelly.Evaluator;

internal class LiteralEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictionaryValue node)
    {
        return node[Keywords.Value];
    }
}