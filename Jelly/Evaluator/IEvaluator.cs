namespace Jelly.Evaluator;

using Jelly.Values;

public interface IEvaluator
{
    Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator);

    Value Evaluate(IScope scope, DictionaryValue node) => Evaluate(scope, node, this);
}