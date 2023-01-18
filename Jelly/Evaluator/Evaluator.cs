using Jelly.Values;

namespace Jelly.Evaluator;

public class Evaluator : IEvaluator
{
    readonly NodeEvaluator _evaluator;

    public Evaluator()
    {
        _evaluator = new NodeEvaluator();
        _evaluator.AddEvaluator("literal", new LiteralEvaluator());
        _evaluator.AddEvaluator("variable", new VariableEvaluator());
        _evaluator.AddEvaluator("command", new CommandEvaluator());
    }

    public Value Evaluate(IScope scope, DictionaryValue node)
    {
        return Evaluate(scope, node, this);
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator)
    {
        return _evaluator.Evaluate(scope, node, this);
    }
}