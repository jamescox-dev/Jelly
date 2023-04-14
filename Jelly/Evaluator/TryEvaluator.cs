namespace Jelly.Evaluator;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Values;

internal class TryEvaluator : IEvaluator
{
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        Value result = Value.Empty;

        try
        {
            result = rootEvaluator.Evaluate(scope, node[Keywords.Body].ToDictionaryValue(), rootEvaluator);
        }
        catch (Error error)
        {
            var handled = false;
            foreach (var errorHandler in node[Keywords.ErrorHandlers].ToListValue())
            {
                var errorHandlerList = errorHandler.ToListValue();
                var patternNode = errorHandlerList[0].ToDictionaryValue();
                if (error.IsType(rootEvaluator.Evaluate(scope, patternNode).ToString()))
                {
                    var handlerBodyNode = errorHandlerList[1].ToDictionaryValue();
                    result = rootEvaluator.Evaluate(scope, handlerBodyNode);
                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                throw;
            }
        }
        finally
        {
            if (node.ContainsKey(Keywords.Finally))
            {
                result = rootEvaluator.Evaluate(scope, node[Keywords.Finally].ToDictionaryValue(), rootEvaluator);
            }
        }

        return result;
    }
}