using Jelly.Values;

namespace Jelly.Evaluator;

public class ExpressionEvaluator : IEvaluator
{
    static readonly StringValue SubexpressionsKeyword = new StringValue("subexpressions");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var result = Value.Empty;
        foreach (var subexpression in node[SubexpressionsKeyword].ToListValue())
        {
            result = rootEvaluator.Evaluate(scope, subexpression.ToDictionaryValue(), rootEvaluator);
        }
        return result;
    }
}