namespace Jelly.Evaluator;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Values;

internal class RaiseEvaluator : IEvaluator
{
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        throw Error.Create(
            rootEvaluator.Evaluate(scope, node[Keywords.ErrorType].ToDictionaryValue()).ToString(),
            rootEvaluator.Evaluate(scope, node[Keywords.Message].ToDictionaryValue()).ToString(),
            rootEvaluator.Evaluate(scope, node[Keywords.Value].ToDictionaryValue()));
    }
}