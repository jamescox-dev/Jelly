namespace Jelly.Evaluator;

internal class AssignmentEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        throw new NotImplementedException();
    }
    
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var value = rootEvaluator.Evaluate(scope, node[Keywords.Value].ToDictionaryValue(), rootEvaluator);
        scope.SetVariable(node[Keywords.Name].ToString(), value);
        return value;
    }
}