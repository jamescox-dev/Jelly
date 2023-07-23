namespace Jelly.Evaluator;

public class ForListEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        var iteratorName = GetIteratorName(env, node);
        var indexName = GetIndexName(env, node);
        var list = GetList(env, node);
        var body = node.GetNode(Keywords.Body);

        AssertIteratorNamesAreUnique(iteratorName, indexName);

        return RunLoop(env, iteratorName, indexName, list, body);
    }

    static Value RunLoop(IEnv env, string iteratorName, string? indexName, ListValue list, DictValue body)
    {
        var result = Value.Empty;
        var index = 1;
        foreach (var keyValue in list)
        {
            PushLoopBodyScope(env, iteratorName, indexName, keyValue, index++);
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

    static string GetIteratorName(IEnv env, DictValue node)
    {
        return env.Evaluate(node.GetNode(Keywords.ItValue)).ToString();
    }

    static string? GetIndexName(IEnv env, DictValue node)
    {
        return node.ContainsKey(Keywords.ItIndex) ? env.Evaluate(node.GetNode(Keywords.ItIndex)).ToString() : null;
    }

    static ListValue GetList(IEnv env, DictValue node)
    {
        return env.Evaluate(node.GetNode(Keywords.List)).ToListValue();
    }

    static void AssertIteratorNamesAreUnique(string iteratorName, string? indexName)
    {
        if (iteratorName.Equals(indexName, StringComparison.InvariantCultureIgnoreCase))
        {
            throw Error.Arg($"index and value iterators can not have the same name '{iteratorName}'.");
        }
    }

    static void PushLoopBodyScope(IEnv env, string iteratorName, string? indexName, Value value, int index)
    {
        env.PushScope();
        env.CurrentScope.DefineVariable(iteratorName, value);
        if (indexName is not null)
        {
            env.CurrentScope.DefineVariable(indexName, index.ToValue());
        }
    }
}