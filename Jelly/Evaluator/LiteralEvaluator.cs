namespace Jelly.Evaluator;

using Jelly.Values;

public class LiteralEvaluator : IEvaluator
{
    static readonly StringValue ValueKey = new StringValue("value");

    public Value Evaluate(Scope scope, DictionaryValue node, IEvaluator interpreter) =>
        node[ValueKey];
}