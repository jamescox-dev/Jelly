using Jelly.Values;

namespace Jelly.Evaluator;

internal class CompositeEvaluator : IEvaluator
{
    static readonly StringValue PartsKeyword = new StringValue("parts");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator)
    {
        return new StringValue(string.Join("", node[PartsKeyword].ToListValue()
            .Select(part => evaluator.Evaluate(scope, part.ToDictionaryValue(), evaluator))));
    }
}