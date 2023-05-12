namespace Jelly.Evaluator;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Values;

public class ForListEvaluator : IEvaluator
{
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var iteratorName = rootEvaluator.Evaluate(scope, node[Keywords.ItValue].ToDictionaryValue()).ToString();
        var indexName = node.ContainsKey(Keywords.ItIndex) ? rootEvaluator.Evaluate(scope, node[Keywords.ItIndex].ToDictionaryValue()).ToString() : null;
        var list = rootEvaluator.Evaluate(scope, node[Keywords.List].ToDictionaryValue()).ToListValue();
        var body = node[Keywords.Body].ToDictionaryValue();

        if (iteratorName.Equals(indexName, StringComparison.InvariantCultureIgnoreCase))
        {
            throw Error.Arg($"index and value interators can not have the same name '{iteratorName}'.");
        }

        Value result = Value.Empty;
        var index = 1;
        foreach (var value in list)
        {
            var innerScope = new Scope(scope);
            innerScope.DefineVariable(iteratorName, value);
            if (indexName is not null)
            {
                innerScope.DefineVariable(indexName, index.ToValue());
                ++index;
            }
            try
            {
                result = rootEvaluator.Evaluate(innerScope, body);
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
}