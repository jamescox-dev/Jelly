using Jelly.Values;

namespace Jelly.Evaluator;

internal class CommandEvaluator : IEvaluator
{
    static readonly StringValue NameKey = new StringValue("name");
    static readonly StringValue ArgsKey = new StringValue("args");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var name = rootEvaluator.Evaluate(scope, node[NameKey].ToDictionaryValue(), rootEvaluator).ToString();
        var command = scope.GetCommand(name);
        var args = command.IsMacro 
            ? node[ArgsKey].ToListValue()
            : node[ArgsKey].ToListValue().Select(arg => rootEvaluator.Evaluate(scope, arg.ToDictionaryValue(), rootEvaluator)).ToValue();
        
        return command.Invoke(scope, args);
    }
}