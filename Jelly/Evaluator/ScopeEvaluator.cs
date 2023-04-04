namespace Jelly.Evaluator;

using Jelly.Ast;
using Jelly.Values;

internal class ScopeEvaluator : IEvaluator
{
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var innerScope = new Scope(scope);
        return rootEvaluator.Evaluate(innerScope, node[Keywords.Body].ToDictionaryValue());
    }
}