namespace Jelly.Evaluator;

public interface IEvaluator
{
    Value Evaluate(IEnv env, DictValue node);
}