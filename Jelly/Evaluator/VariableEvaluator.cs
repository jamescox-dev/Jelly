namespace Jelly.Evaluator;

internal class VariableEvaluator : IEvaluator
{
    static readonly StringValue NameKey = new("name");

    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        throw new NotImplementedException();
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator) =>
        scope.GetVariable(node[NameKey].ToString());
}