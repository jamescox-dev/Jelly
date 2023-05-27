namespace Jelly.Evaluator;

public class ForDictEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var iteratorName = GetIteratorName(env, node);
        var keyName = GetKeyName(env, node);
        var dict = GetDict(env, node);
        var body = node.GetNode(Keywords.Body);

        AssertIteratorNamesAreUnique(iteratorName, keyName);

        return RunLoop(env, iteratorName, keyName, dict, body);
    }

    static Value RunLoop(IEnvironment env, string iteratorName, string? keyName, DictionaryValue dict, DictionaryValue body)
    {
        var result = Value.Empty;
        foreach (var keyValue in dict.ToEnumerable())
        {
            PushLoopBodyScope(env, iteratorName, keyName, keyValue);
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

    static string GetIteratorName(IEnvironment env, DictionaryValue node)
    {
        return env.Evaluate(node.GetNode(Keywords.ItValue)).ToString();
    }

    static string? GetKeyName(IEnvironment env, DictionaryValue node)
    {
        return node.ContainsKey(Keywords.ItKey) ? env.Evaluate(node.GetNode(Keywords.ItKey)).ToString() : null;
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

    static void PushLoopBodyScope(IEnvironment env, string iteratorName, string? keyName, KeyValuePair<Value, Value> keyValue)
    {
        env.PushScope();
        env.CurrentScope.DefineVariable(iteratorName, keyValue.Value);
        if (keyName is not null)
        {
            env.CurrentScope.DefineVariable(keyName, keyValue.Key);
        }
    }
}