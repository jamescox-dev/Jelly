namespace Jelly.Evaluator;

internal class DefineVariableEvaluator : IEvaluator
{
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var value = rootEvaluator.Evaluate(scope, node[Keywords.Value].ToDictionaryValue(), rootEvaluator);
        scope.DefineVariable(node[Keywords.Name].ToString(), value);
        return value;
    }
}