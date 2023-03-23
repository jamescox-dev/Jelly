namespace Jelly.Evaluator;

using Jelly.Ast;
using Jelly.Values;

internal class WhileEvaluator : IEvaluator
{
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var result = Value.Empty;
        
        while (EvaluateComdition(scope, node, rootEvaluator))
        {
            result = EvaluateBody(scope, node, rootEvaluator);
        }

        return result;
    }

    private static Value EvaluateBody(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        return rootEvaluator.Evaluate(scope, node[Keywords.Body].ToDictionaryValue());
    }

    private static bool EvaluateComdition(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        return rootEvaluator.Evaluate(scope, node[Keywords.Condition].ToDictionaryValue()).ToBool();
    }
}