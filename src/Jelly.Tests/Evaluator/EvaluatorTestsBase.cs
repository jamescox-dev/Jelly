namespace Jelly.Evaluator.Tests;

using Jelly.Runtime;

public abstract class EvaluatorTestsBase
{
    protected IEvaluator Evaluator { get; private set; } = null!;

    protected Env Environment { get; private set; } = null!;

    protected Value Evaluate(DictValue node) => Evaluator.Evaluate(Environment, node);

    [SetUp]
    public virtual void Setup()
    {
        Environment = new Env();
        Evaluator = BuildEvaluatorUnderTest();
    }

    protected abstract IEvaluator BuildEvaluatorUnderTest();
}