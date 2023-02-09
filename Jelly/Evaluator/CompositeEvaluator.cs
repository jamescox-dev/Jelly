using Jelly.Values;

namespace Jelly.Evaluator;

internal class CompositeEvaluator : IEvaluator
{
    static readonly StringValue PartsKeyword = new StringValue("parts");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        return new StringValue(string.Join("", node[PartsKeyword].ToListValue()
            .Select(part => rootEvaluator.Evaluate(scope, part.ToDictionaryValue(), rootEvaluator))));
    }
}