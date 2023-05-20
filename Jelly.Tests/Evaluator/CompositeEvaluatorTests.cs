namespace Jelly.Evaluator.Tests;

[TestFixture]
public class CompositeEvaluatorTests
{
    [Test]
    public void TheResultOfEvaluatingACompositeNodeIsTheResultOfEvaluatingEachOfItsPartsAndConcatenatingTheResults()
    {
        var compositeEvaluator = new CompositeEvaluator();
        var evaluator = new LiteralEvaluator();
        var part1 = Node.Literal("jello,".ToValue());
        var part2 = Node.Literal(" world".ToValue());
        var node = Node.Composite(part1, part2);

        var result = compositeEvaluator.Evaluate(new Mock<IScope>().Object, node, evaluator);

        result.Should().Be("jello, world".ToValue());
    }

    [Test]
    public void TheResultOfEvaluatingACompositeNodeThatHasNoPartsIsAnEmptyValue()
    {
        var compositeEvaluator = new CompositeEvaluator();
        var node = Node.Composite();

        var result = compositeEvaluator.Evaluate(new Mock<IScope>().Object, node, compositeEvaluator);

        result.Should().Be(Value.Empty);
    }
}