namespace Jelly.Evaluator;

using Jelly.Values;

internal class LiteralEvaluator : IEvaluator
{
    static readonly StringValue ValueKey = new StringValue("value");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator) =>
        node[ValueKey];
}