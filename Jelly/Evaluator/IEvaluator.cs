namespace Jelly.Evaluator;

public interface IEvaluator
{
    Value Evaluate(IEnv env, DictionaryValue node);
}