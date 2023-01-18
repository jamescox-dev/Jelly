using Jelly.Values;

namespace Jelly.Evaluator;

internal class CommandEvaluator : IEvaluator
{
    static readonly StringValue NameKey = new StringValue("name");
    static readonly StringValue ArgsKey = new StringValue("args");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator)
    {
        var name = evaluator.Evaluate(scope, node[NameKey].ToDictionaryValue(), evaluator).ToString();
        var command = scope.GetCommand(name);
        var args = command.IsMacro 
            ? node[ArgsKey].ToListValue()
            : node[ArgsKey].ToListValue().Select(arg => evaluator.Evaluate(scope, arg.ToDictionaryValue(), evaluator)).ToValue();
        
        return command.Invoke(scope, args);
    }
}