namespace Jelly.Evaluator.Tests;

using Jelly.Ast;
using Jelly.Values;

[TestFixture]
public class ExpressionEvaluatorTests
{
    IEvaluator _evaluator = null!;
    
    Evaluator _rootEvaluator = null!;
    Scope _scope = null!;

    [Test]
    public void AnExpressionEvaluatesItsRootNode()
    {
        var expr = Node.Expression(Node.Literal("42".ToValue()));

        var result = _evaluator.Evaluate(_scope, expr, _rootEvaluator);

        result.Should().Be("42".ToValue());
    }

    [SetUp]
    public void Setup()
    {
        _rootEvaluator = new Evaluator();
        _scope = new Scope();

        _evaluator = new ExpressionEvaluator();
    }
}