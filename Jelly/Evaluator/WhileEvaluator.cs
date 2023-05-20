namespace Jelly.Evaluator;

internal class WhileEvaluator : IEvaluator
{
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var result = Value.Empty;

        while (EvaluateComdition(scope, node, rootEvaluator))
        {
            try
            {
                result = EvaluateBody(scope, node, rootEvaluator);
            }
            catch (Break)
            {
                result = Value.Empty;
                break;
            }
            catch (Continue)
            {
                continue;
            }
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