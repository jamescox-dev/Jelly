

namespace Jelly.Evaluator;

internal class ScriptEvaluator : IEvaluator
{
    static readonly StringValue CommandsKeyword = new StringValue("commands");

    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var commands = node.GetList(Keywords.Commands);

        var result = Value.Empty;
        foreach (var command in commands)
        {
            result = env.Evaluate(command.ToNode());
        }

        return result;
    }
}