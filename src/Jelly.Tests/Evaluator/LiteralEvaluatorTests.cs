namespace Jelly.Tests.Evaluator;

[TestFixture]
public class LiteralEvaluatorTests
{
    [Test]
    public void EvaluatingALiteralNodeReturnsTheNodesValue()
    {
        var evaluator = new LiteralEvaluator();
        var literal = Node.Literal("hello, world");

        var result = evaluator.Evaluate(null!, literal);

        result.Should().Be(new StrValue("hello, world"));
    }
}