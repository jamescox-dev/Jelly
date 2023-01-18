using Jelly.Values;

namespace Jelly.Evaluator;

internal class CommandEvaluator : IEvaluator
{
    static readonly StringValue NameKey = new StringValue("name");
    static readonly StringValue ArgsKey = new StringValue("args");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator)
    {
        var name = evaluator.Evaluate(scope, node[NameKey].AsDictionary(), evaluator).ToString();
        var command = scope.GetCommand(name);
        var args = node[ArgsKey].AsList().Select(arg => evaluator.Evaluate(scope, arg.AsDictionary(), evaluator)).ToValue();
        
        return command.Invoke(scope, args);
    }
}