namespace Jelly.Evaluator;

using Jelly.Values;

internal class VariableEvaluator : IEvaluator
{
    static readonly StringValue NameKey = new("name");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator) =>
        scope.GetVariable(node[NameKey].ToString());
}