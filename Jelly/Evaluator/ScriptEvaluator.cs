

namespace Jelly.Evaluator;

internal class ScriptEvaluator : IEvaluator
{
    static readonly StringValue CommandsKeyword = new StringValue("commands");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var commands = node[CommandsKeyword].ToListValue();

        var result = Value.Empty;
        foreach (var command in commands)
        {
            result = rootEvaluator.Evaluate(scope, command.ToDictionaryValue(), rootEvaluator);
        }

        return result;
    }
}