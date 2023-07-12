namespace Jelly.Evaluator;

internal class DefineVariableEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictionaryValue node)
    {
        var name = node.GetString(Keywords.Name);
        var value = env.Evaluate(node.GetNode(Keywords.Value));
        env.CurrentScope.DefineVariable(name, value);
        return value;
    }
}