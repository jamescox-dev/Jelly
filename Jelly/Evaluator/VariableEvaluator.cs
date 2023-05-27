namespace Jelly.Evaluator;

internal class VariableEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node) =>
        env.CurrentScope.GetVariable(node.GetString(Keywords.Name));
}