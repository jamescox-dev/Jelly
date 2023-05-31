namespace Jelly.Evaluator;

internal class AssignmentEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var name = node.GetString(Keywords.Name);
        var value = env.Evaluate(node.GetNode(Keywords.Value));
        env.CurrentScope.SetVariable(name, value);
        return value;
    }
}