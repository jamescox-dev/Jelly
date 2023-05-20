namespace Jelly.Evaluator;

public class ForDictEvaluator : IEvaluator
{
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var iteratorName = rootEvaluator.Evaluate(scope, node[Keywords.ItValue].ToDictionaryValue()).ToString();
        var keyName = node.ContainsKey(Keywords.ItKey) ? rootEvaluator.Evaluate(scope, node[Keywords.ItKey].ToDictionaryValue()).ToString() : null;
        var dict = rootEvaluator.Evaluate(scope, node[Keywords.Dict].ToDictionaryValue()).ToDictionaryValue();
        var body = node[Keywords.Body].ToDictionaryValue();

        if (iteratorName.Equals(keyName, StringComparison.InvariantCultureIgnoreCase))
        {
            throw Error.Arg($"key and value interators can not have the same name '{iteratorName}'.");
        }

        Value result = Value.Empty;
        foreach (var keyValue in dict.ToEnumerable())
        {
            var innerScope = new Scope(scope);
            innerScope.DefineVariable(iteratorName, keyValue.Value);
            if (keyName is not null)
            {
                innerScope.DefineVariable(keyName, keyValue.Key);
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