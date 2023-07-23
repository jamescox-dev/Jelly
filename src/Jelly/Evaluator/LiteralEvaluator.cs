namespace Jelly.Evaluator;

internal class LiteralEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        return node[Keywords.Value];
    }
}