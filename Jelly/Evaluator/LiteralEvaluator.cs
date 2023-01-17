namespace Jelly.Evaluator;

public class LiteralEvaluator : IEvaluator
{
    static readonly StringValue ValueKey = new StringValue("value");

    public Value Evaluate(Scope scope, DictionaryValue node, IEvaluator interpreter) =>
        node[ValueKey];
}