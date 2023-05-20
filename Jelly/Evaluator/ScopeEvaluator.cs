namespace Jelly.Evaluator;

internal class ScopeEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        throw new NotImplementedException();
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var innerScope = new Scope(scope);
        return rootEvaluator.Evaluate(innerScope, node[Keywords.Body].ToDictionaryValue());
    }
}