namespace Jelly.Evaluator;

public interface IEvaluator
{
    Value Evaluate(IEnvironment env, DictionaryValue node);
}