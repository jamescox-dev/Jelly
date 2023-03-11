namespace Jelly.Evaluator.Tests;

using Jelly.Ast;
using Jelly.Values;

[TestFixture]
public class BinOpEvaluatorTests
{
    IEvaluator _evaluator = null!;
    
    Evaluator _rootEvaluator = null!;
    Scope _scope = null!;

    [Test]
    public void AdditionsCanBeEvaluated()
    {
        var binOp = Node.BinOp("add", Node.Literal(3), Node.Literal(7));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be(10.0.ToValue());
    }

    [Test]
    public void SubtractionsCanBeEvaluated()
    {
        var binOp = Node.BinOp("sub", Node.Literal(5), Node.Literal(11));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be((-6.0).ToValue());
    }

    [Test]
    public void MultiplicationsCanBeEvaluated()
    {
        var binOp = Node.BinOp("mul", Node.Literal(5), Node.Literal(11));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be(55.0.ToValue());
    }

    [Test]
    public void DivisionsCanBeEvaluated()
    {
        var binOp = Node.BinOp("div", Node.Literal(10), Node.Literal(2));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be(5.0.ToValue());
    }

    [TestCase(10.0, 3.0, 3.0)]
    [TestCase(-10.0, 3.0, -4.0)]
    [TestCase(10.0, -3.0, -4.0)]
    [TestCase(-10.0, -3.0, 3.0)]
    public void AFlooredDivisionsCanBeEvaluated(double a, double b, double c)
    {
        var binOp = Node.BinOp("floordiv", Node.Literal(a), Node.Literal(b));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be(c.ToValue());
    }

    [TestCase(7.0, 3.0, 1.0)]
    [TestCase(-7.0, 3.0, 2.0)]
    [TestCase(7.0, -3.0, -2.0)]
    [TestCase(-7.0, -3.0, -1.0)]
    public void AModuloCanBeEvaluated(double a, double b, double c)
    {
        var binOp = Node.BinOp("mod", Node.Literal(a), Node.Literal(b));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be(c.ToValue());
    }

    [TestCase(7.5, 3.0, 1.0)]
    [TestCase(-7.5, 3.0, 1.0)]
    [TestCase(7.5, -3.0, -2.0)]
    [TestCase(-7.5, -3.0, -2.0)]
    public void AFlooredModuloCanBeEvaluated(double a, double b, double c)
    {
        var binOp = Node.BinOp("floormod", Node.Literal(a), Node.Literal(b));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be(c.ToValue());
    }

    [Test]
    public void ExponentiationCanBeEvaluated()
    {
        var binOp = Node.BinOp("exp", Node.Literal(10), Node.Literal(2));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be(100.0.ToValue());
    }

    [Test]
    public void LessThanCanBeEvaluated()
    {
        var binOp = Node.BinOp("lt", Node.Literal(2), Node.Literal(10));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be(true.ToValue());
    }

    [Test]
    public void GreaterThanCanBeEvaluated()
    {
        var binOp = Node.BinOp("gt", Node.Literal(2), Node.Literal(10));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        result.Should().Be(false.ToValue());
    }

    // TODO:  Implement more operators.

    // TODO:  Test for unknown binary-operators.

    [TestCase(1.0, 0.0, 1)]
    [TestCase(-1.0, 0.0, -1)]
    [TestCase(1.0, -0.0, -1)]
    [TestCase(-1.0, -0.0, 1)]
    public void DivisionsByZeroEvaluateToInfinityWithMatchingSign(double a, double zero, int sign)
    {
        var binOp = Node.BinOp("div", Node.Literal(a), Node.Literal(zero));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator).ToDouble();

        double.IsInfinity(result).Should().BeTrue();
        if (sign > 0)
        {
            result.Should().BeGreaterThan(0);
        }
        else
        {
            result.Should().BeLessThan(0);
        }
    }

    [TestCase("add", double.NaN, 1)]
    [TestCase("sub", 1, double.NaN)]
    [TestCase("mul", double.NaN, double.NaN)]
    [TestCase("div", double.NaN, 1)]
    [TestCase("add", 1, double.NaN)]
    [TestCase("sub", double.NaN, double.NaN)]
    [TestCase("floordiv", double.NaN, double.NaN)]
    public void IfAnyOfTheOperandsOfAnArithmaticOperatorAreNaNTheResultIsNaN(string op, double a, double b)
    {
        var binOp = Node.BinOp(op, Node.Literal(a), Node.Literal(b));

        var result = _evaluator.Evaluate(_scope, binOp, _rootEvaluator);

        double.IsNaN(result.ToDouble()).Should().BeTrue();
    }

    [SetUp]
    public void Setup()
    {
        _rootEvaluator = new Evaluator();
        _scope = new Scope();

        _evaluator = new BinOpEvaluator();
    }
}