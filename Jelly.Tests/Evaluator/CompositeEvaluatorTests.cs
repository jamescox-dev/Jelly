namespace Jelly.Evaluator.Tests;

using Jelly.Parser;
using Jelly.Values;

[TestFixture]
public class CompositeEvaluatorTests
{
    [Test]
    public void TheResultOfEvaluatingACompositeNodeIsTheResultOfEvaluatingEachOfItsPartsAndConcatenatingTheResults()
    {
        var compositeEvaluator = new CompositeEvaluator();
        var evaluator = new LiteralEvaluator();
        var builder = new NodeBuilder();
        var part1 = builder.Literal("jello,".ToValue());
        var part2 = builder.Literal(" world".ToValue());
        var node = builder.Composite(part1, part2);

        var result = compositeEvaluator.Evaluate(new Mock<IScope>().Object, node, evaluator);

        result.Should().Be("jello, world".ToValue());
    }

    [Test]
    public void TheResultOfEvaluatingACompositeNodeThatHasNoPartsIsAnEmptyValue()
    {
        var compositeEvaluator = new CompositeEvaluator();
        var node = new NodeBuilder().Composite();

        var result = compositeEvaluator.Evaluate(new Mock<IScope>().Object, node, compositeEvaluator);

        result.Should().Be(Value.Empty);
    }
}