using Jelly.Values;

namespace Jelly.Evaluator;

public class Evaluator : IEvaluator
{
    readonly NodeEvaluator _evaluator;

    public static readonly IEvaluator Shared = new Evaluator();

    public Evaluator()
    {
        _evaluator = new NodeEvaluator();
        _evaluator.AddEvaluator("literal", new LiteralEvaluator());
        _evaluator.AddEvaluator("variable", new VariableEvaluator());
        _evaluator.AddEvaluator("command", new CommandEvaluator());
        _evaluator.AddEvaluator("script", new ScriptEvaluator());
        _evaluator.AddEvaluator("assignment", new AssignmentEvaluator());
        _evaluator.AddEvaluator("composite", new CompositeEvaluator());
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator)
    {
        return _evaluator.Evaluate(scope, node, this);
    }
}