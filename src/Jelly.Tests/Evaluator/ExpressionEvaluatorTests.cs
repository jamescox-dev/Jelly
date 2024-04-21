namespace Jelly.Tests.Evaluator;

[TestFixture]
public class ExpressionEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void AnExpressionEvaluatesEachOfItsSubExpressionsNode()
    {
        var invocations = 0;
        var testCommand = new SimpleCommand((_) => { ++invocations; return invocations.ToValue(); });
        Environment.GlobalScope.DefineCommand("test", testCommand);
        var expr = Node.Expression
        (Node.Command(Node.Literal("test"), new ListValue()),
            Node.Command(Node.Literal("test"), new ListValue()));

        var result = Evaluate(expr);

        invocations.Should().Be(2);
        result.Should().Be(2.0.ToValue());
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new ExpressionEvaluator();
    }
}