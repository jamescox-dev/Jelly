using Jelly.Values;

namespace Jelly.Evaluator;

public class ExpressionEvaluator : IEvaluator
{
    static readonly StringValue RootKeyword = new StringValue("root");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        return rootEvaluator.Evaluate(scope, node[RootKeyword].ToDictionaryValue(), rootEvaluator);
    }
}