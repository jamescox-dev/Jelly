namespace Jelly.Evaluator;

internal class RaiseEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        throw Error.Create(
            env.Evaluate(node.GetNode(Keywords.ErrorType)).ToString(),
            env.Evaluate(node.GetNode(Keywords.Message)).ToString(),
            env.Evaluate(node.GetNode(Keywords.Value)));
    }
}