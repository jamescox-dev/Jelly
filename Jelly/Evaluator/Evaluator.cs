namespace Jelly.Evaluator;

using Jelly.Ast;
using Jelly.Values;

public class Evaluator : IEvaluator
{
    readonly NodeEvaluator _evaluator;

    public static readonly IEvaluator Shared = new Evaluator();

    public Evaluator()
    {
        _evaluator = new NodeEvaluator();
        _evaluator.AddEvaluator(Keywords.Literal.ToString(), new LiteralEvaluator());
        _evaluator.AddEvaluator(Keywords.Variable.ToString(), new VariableEvaluator());
        _evaluator.AddEvaluator(Keywords.Command.ToString(), new CommandEvaluator());
        _evaluator.AddEvaluator(Keywords.Script.ToString(), new ScriptEvaluator());
        _evaluator.AddEvaluator(Keywords.Assignment.ToString(), new AssignmentEvaluator());
        _evaluator.AddEvaluator(Keywords.Composite.ToString(), new CompositeEvaluator());
        _evaluator.AddEvaluator(Keywords.Expression.ToString(), new ExpressionEvaluator());
        _evaluator.AddEvaluator(Keywords.BinOp.ToString(), new BinOpEvaluator());
        _evaluator.AddEvaluator(Keywords.UniOp.ToString(), new UnaryOpEvaluator());
        _evaluator.AddEvaluator(Keywords.DefineVariable.ToString(), new DefineVariableEvaluator());
        _evaluator.AddEvaluator(Keywords.If.ToString(), new IfEvaluator());
        _evaluator.AddEvaluator(Keywords.While.ToString(), new WhileEvaluator());
        _evaluator.AddEvaluator(Keywords.Scope.ToString(), new ScopeEvaluator());
        _evaluator.AddEvaluator(Keywords.Raise.ToString(), new RaiseEvaluator());
        _evaluator.AddEvaluator(Keywords.Try.ToString(), new TryEvaluator());
        _evaluator.AddEvaluator(Keywords.DefineCommand.ToString(), new DefineCommandEvaluator());
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        return _evaluator.Evaluate(scope, node, this);
    }
}