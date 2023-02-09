namespace Jelly.Evaluator;

using Jelly.Values;

internal class AssignmentEvaluator : IEvaluator
{
    static readonly StringValue NameKeyword = new StringValue("name");
    static readonly StringValue ValueKeyword = new StringValue("value");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var value = rootEvaluator.Evaluate(scope, node[ValueKeyword].ToDictionaryValue(), rootEvaluator);
        scope.SetVariable(node[NameKeyword].ToString(), value);
        return value;
    }
}