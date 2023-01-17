namespace Jelly.Evaluator;

using Jelly.Values;

public interface IEvaluator
{
    Value Evaluate(Scope scope, DictionaryValue node, IEvaluator interpreter);
}