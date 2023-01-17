namespace Jelly.Evaluator;

public interface IEvaluator
{
    Value Evaluate(Scope scope, DictionaryValue node, IEvaluator interpreter);
}