namespace Jelly.Evaluator;

internal class RaiseEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        throw new NotImplementedException();
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        throw Error.Create(
            Error.NormalizeType(rootEvaluator.Evaluate(scope, node[Keywords.ErrorType].ToDictionaryValue()).ToString()),
            rootEvaluator.Evaluate(scope, node[Keywords.Message].ToDictionaryValue()).ToString(),
            rootEvaluator.Evaluate(scope, node[Keywords.Value].ToDictionaryValue()));
    }
}