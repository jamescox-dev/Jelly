namespace Jelly.Evaluator.Tests;

using Jelly.Runtime;

public abstract class EvaluatorTestsBase
{
    protected IEvaluator Evaluator { get; private set; } = null!;

    protected Environment Environment { get; private set; } = null!;

    protected Value Evaluate(DictionaryValue node) => Evaluator.Evaluate(Environment, node);

    [SetUp]
    public virtual void Setup()
    {
        Environment = new Environment();
        Evaluator = BuildEvaluatorUnderTest();
    }

    protected abstract IEvaluator BuildEvaluatorUnderTest();
}