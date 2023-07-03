namespace Jelly.Evaluator;

public class ForDictEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var keyIteratorName = GetKeyIteratorName(env, node);
        var valueIteratorName = GetValueIteratorName(env, node);
        var dict = GetDict(env, node);
        var body = node.GetNode(Keywords.Body);

        AssertIteratorNamesAreUnique(keyIteratorName, valueIteratorName);

        return RunLoop(env, keyIteratorName, valueIteratorName, dict, body);
    }

    static Value RunLoop(IEnvironment env, string keyIteratorName, string? valueIteratorName, DictionaryValue dict, DictionaryValue body)
    {
        var result = Value.Empty;
        foreach (var keyValue in dict.ToEnumerable())
        {
            PushLoopBodyScope(env, keyIteratorName, valueIteratorName, keyValue);
            try
            {
                result = env.Evaluate(body);
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
            finally
            {
                env.PopScope();
            }
        }
        return result;
    }

    static string GetKeyIteratorName(IEnvironment env, DictionaryValue node)
    {
        return env.Evaluate(node.GetNode(Keywords.ItKey)).ToString();
    }

    static string? GetValueIteratorName(IEnvironment env, DictionaryValue node)
    {
        return node.ContainsKey(Keywords.ItValue) ? env.Evaluate(node.GetNode(Keywords.ItValue)).ToString() : null;
    }

    static DictionaryValue GetDict(IEnvironment env, DictionaryValue node)
    {
        return env.Evaluate(node.GetNode(Keywords.Dict)).ToDictionaryValue();
    }

    static void AssertIteratorNamesAreUnique(string iteratorName, string? keyName)
    {
        if (iteratorName.Equals(keyName, StringComparison.InvariantCultureIgnoreCase))
        {
            throw Error.Arg($"key and value iterators can not have the same name '{iteratorName}'.");
        }
    }

    static void PushLoopBodyScope(IEnvironment env, string keyIteratorName, string? valueIteratorName, KeyValuePair<Value, Value> keyValue)
    {
        env.PushScope();
        env.CurrentScope.DefineVariable(keyIteratorName, keyValue.Key);
        if (valueIteratorName is not null)
        {
            env.CurrentScope.DefineVariable(valueIteratorName, keyValue.Value);
        }
    }
}