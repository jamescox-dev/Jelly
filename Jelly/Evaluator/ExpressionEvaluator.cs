

namespace Jelly.Evaluator;

public class ExpressionEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var result = Value.Empty;
        foreach (var subExpression in node.GetList(Keywords.SubExpressions))
        {
            result = env.Evaluate(subExpression.ToNode());
        }
        return result;
    }
}