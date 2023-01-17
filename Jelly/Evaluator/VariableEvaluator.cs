namespace Jelly.Evaluator;

public class VariableEvaluator : IEvaluator
{
    static readonly StringValue NameKey = new("name");

    public Value Evaluate(Scope scope, DictionaryValue node, IEvaluator interpreter) =>
        scope.GetVariable(node[NameKey].ToString());
}