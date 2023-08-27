namespace Jelly.Evaluator.Tests;

[TestFixture]
public class CompositeEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheResultOfEvaluatingACompositeNodeIsTheResultOfEvaluatingEachOfItsPartsAndConcatenatingTheResults()
    {
        var part1 = Node.Literal("jello,".ToValue());
        var part2 = Node.Literal(" world".ToValue());
        var composite = Node.Composite(part1, part2);

        var result = Evaluate(composite);

        result.Should().Be("jello, world".ToValue());
    }

    [Test]
    public void TheResultOfEvaluatingACompositeNodeThatHasNoPartsIsAnEmptyValue()
    {
        var composite = Node.Composite();

        var result = Evaluate(composite);

        result.Should().Be(Value.Empty);
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new CompositeEvaluator();
    }
}