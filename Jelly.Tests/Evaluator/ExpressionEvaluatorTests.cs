namespace Jelly.Evaluator.Tests;

[TestFixture]
public class ExpressionEvaluatorTests
{
    IEvaluator _evaluator = null!;

    Evaluator _rootEvaluator = null!;
    Scope _scope = null!;

    [Test]
    public void AnExpressionEvaluatesEachOfItsSubExpressionsNode()
    {
        var invokations = 0;
        var testCommand = new SimpleCommand((_, _) => { ++invokations; return invokations.ToValue(); });
        _scope.DefineCommand("test", testCommand);
        var expr = Node.Expression
        (Node.Command(Node.Literal("test"), new ListValue()),
            Node.Command(Node.Literal("test"), new ListValue()));

        var result = _evaluator.Evaluate(_scope, expr, _rootEvaluator);

        invokations.Should().Be(2);
        result.Should().Be(2.0.ToValue());
    }


    [SetUp]
    public void Setup()
    {
        _rootEvaluator = new Evaluator();
        _scope = new Scope();

        _evaluator = new ExpressionEvaluator();
    }
}