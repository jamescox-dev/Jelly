using Jelly.Values;

namespace Jelly.Evaluator;

internal class ScriptEvaluator : IEvaluator
{
    static readonly StringValue CommandsKeyword = new StringValue("commands");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator)
    {
        var commands = node[CommandsKeyword].ToListValue();

        var result = Value.Empty;
        foreach (var command in commands)
        {
            result = evaluator.Evaluate(scope, command.ToDictionaryValue(), evaluator);
        }

        return result;
    }
}