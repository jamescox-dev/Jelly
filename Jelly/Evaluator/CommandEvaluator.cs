namespace Jelly.Evaluator;

internal class CommandEvaluator : IEvaluator
{
    static readonly StringValue NameKey = new StringValue("name");
    static readonly StringValue ArgsKey = new StringValue("args");

    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        throw new NotImplementedException();
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var name = rootEvaluator.Evaluate(scope, node[NameKey].ToDictionaryValue(), rootEvaluator).ToString();
        var command = scope.GetCommand(name);
        var args = (command.EvaluationFlags & EvaluationFlags.Arguments) != 0
            ? node[ArgsKey].ToListValue().Select(arg => rootEvaluator.Evaluate(scope, arg.ToDictionaryValue(), rootEvaluator)).ToValue()
            : node[ArgsKey].ToListValue();

        var result = command.Invoke(scope, args);

        return (command.EvaluationFlags & EvaluationFlags.RetrunValue) != 0
            ? rootEvaluator.Evaluate(scope, result.ToDictionaryValue(), rootEvaluator)
            : result;
    }
}