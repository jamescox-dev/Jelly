namespace Jelly.Evaluator;

internal class LiteralEvaluator : IEvaluator
{
    static readonly StringValue ValueKey = new StringValue("value");

    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        throw new NotImplementedException();
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator) =>
        node[ValueKey];
}