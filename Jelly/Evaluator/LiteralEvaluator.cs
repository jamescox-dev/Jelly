namespace Jelly.Evaluator;

internal class LiteralEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        return node[Keywords.Value];
    }
}