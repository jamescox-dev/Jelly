namespace Jelly.Evaluator.Tests;

[TestFixture]
public class ExpressionEvaluatorTests : EvaluatorTestsBase
{
    IEvaluator _evaluator = null!;

    Evaluator _rootEvaluator = null!;
    Scope _scope = null!;

    [Test]
    public void AnExpressionEvaluatesEachOfItsSubExpressionsNode()
    {
        var invocations = 0;
        var testCommand = new SimpleCommand((_, _) => { ++invocations; return invocations.ToValue(); });
        _scope.DefineCommand("test", testCommand);
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