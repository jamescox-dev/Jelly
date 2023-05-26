namespace Jelly.Evaluator;

internal class WhileEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var condition = node.GetNode(Keywords.Condition);
        var body = node.GetNode(Keywords.Body);

        var result = Value.Empty;
        while (env.Evaluate(condition).ToBool())
        {
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
        }

        return result;
    }
}