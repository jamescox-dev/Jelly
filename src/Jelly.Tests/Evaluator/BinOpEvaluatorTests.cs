namespace Jelly.Evaluator.Tests;

using Jelly.Runtime;

[TestFixture]
public class BinOpEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void ConcatenationsCanBeEvaluated()
    {
        var binOp = Node.BinOp("cat", Node.Literal("jello, "), Node.Literal("world"));

        var result = Evaluate(binOp);

        result.Should().Be("jello, world".ToValue());
    }

    [Test]
    public void AdditionsCanBeEvaluated()
    {
        var binOp = Node.BinOp("add", Node.Literal(3), Node.Literal(7));

        var result = Evaluate(binOp);

        result.Should().Be(10.0.ToValue());
    }

    [Test]
    public void SubtractionsCanBeEvaluated()
    {
        var binOp = Node.BinOp("sub", Node.Literal(5), Node.Literal(11));

        var result = Evaluate(binOp);

        result.Should().Be((-6.0).ToValue());
    }

    [Test]
    public void MultiplicationsCanBeEvaluated()
    {
        var binOp = Node.BinOp("mul", Node.Literal(5), Node.Literal(11));

        var result = Evaluate(binOp);

        result.Should().Be(55.0.ToValue());
    }

    [Test]
    public void DivisionsCanBeEvaluated()
    {
        var binOp = Node.BinOp("div", Node.Literal(10), Node.Literal(2));

        var result = Evaluate(binOp);

        result.Should().Be(5.0.ToValue());
    }

    [TestCase(10.0, 3.0, 3.0)]
    [TestCase(-10.0, 3.0, -4.0)]
    [TestCase(10.0, -3.0, -4.0)]
    [TestCase(-10.0, -3.0, 3.0)]
    public void AFlooredDivisionsCanBeEvaluated(double a, double b, double c)
    {
        var binOp = Node.BinOp("floordiv", Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        result.Should().Be(c.ToValue());
    }

    [TestCase(7.0, 3.0, 1.0)]
    [TestCase(-7.0, 3.0, 2.0)]
    [TestCase(7.0, -3.0, -2.0)]
    [TestCase(-7.0, -3.0, -1.0)]
    public void AModuloCanBeEvaluated(double a, double b, double c)
    {
        var binOp = Node.BinOp("mod", Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        result.Should().Be(c.ToValue());
    }

    [TestCase(7.5, 3.0, 1.0)]
    [TestCase(-7.5, 3.0, 1.0)]
    [TestCase(7.5, -3.0, -2.0)]
    [TestCase(-7.5, -3.0, -2.0)]
    public void AFlooredModuloCanBeEvaluated(double a, double b, double c)
    {
        var binOp = Node.BinOp("floormod", Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        result.Should().Be(c.ToValue());
    }

    [Test]
    public void ExponentiationCanBeEvaluated()
    {
        var binOp = Node.BinOp("exp", Node.Literal(10), Node.Literal(2));

        var result = Evaluate(binOp);

        result.Should().Be(100.0.ToValue());
    }

    [TestCase("lt", 1.0, 2.0, true)]
    [TestCase("lt", 2.0, 2.0, false)]
    [TestCase("lte", 1.0, 2.0, true)]
    [TestCase("lte", 2.0, 2.0, true)]
    [TestCase("lte", 3.0, 2.0, false)]
    [TestCase("eq", 1.0, 1.0, true)]
    [TestCase("eq", 1.0, 0.0, false)]
    [TestCase("gte", 0.0, 1.0, false)]
    [TestCase("gte", 1.0, 1.0, true)]
    [TestCase("gte", 2.0, 1.0, true)]
    [TestCase("gt", 2.0, 1.0, true)]
    [TestCase("gt", 1.0, 1.0, false)]
    [TestCase("ne", 1.0, 1.0, false)]
    [TestCase("ne", 0.0, 1.0, true)]
    public void ComparisonsCanBeEvaluated(string op, double a, double b, bool expected)
    {
        var binOp = Node.BinOp(op, Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        result.Should().Be(expected.ToValue());
    }

    [TestCase("strne", "cat", "dog", true)]
    [TestCase("strne", "cat", "cat", false)]
    [TestCase("streq", "dog", "cat", false)]
    [TestCase("streq", "dog", "dog", true)]
    public void StringComparisonsCanBeEvaluated(string op, string a, string b, bool expected)
    {
        var binOp = Node.BinOp(op, Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        result.Should().Be(expected.ToValue());
    }

    [TestCase("bitor", 2.0, 5.0, 7.0)]
    [TestCase("bitor", double.NaN, 1.0, 1.0)]
    [TestCase("bitor", 2.0, double.NaN, 2.0)]
    [TestCase("bitor", double.NaN, double.NaN, 0.0)]
    [TestCase("bitor", double.PositiveInfinity, 1.0, 1.0)]
    [TestCase("bitor", 2.0, double.PositiveInfinity, 2.0)]
    [TestCase("bitor", double.PositiveInfinity, double.PositiveInfinity, 0.0)]
    [TestCase("bitor", 0xffffffff, 0.0, -1.0)]
    [TestCase("bitor", -1, 0.0, -1.0)]
    [TestCase("bitand", 6.0, 10.0, 2.0)]
    [TestCase("bitand", double.NaN, 1.0, 0.0)]
    [TestCase("bitand", 2.0, double.NaN, 0.0)]
    [TestCase("bitand", double.NaN, double.NaN, 0.0)]
    [TestCase("bitand", double.PositiveInfinity, 1.0, 0.0)]
    [TestCase("bitand", 2.0, double.PositiveInfinity, 0.0)]
    [TestCase("bitand", double.PositiveInfinity, double.PositiveInfinity, 0.0)]
    [TestCase("bitxor", 6.0, 10.0, 12.0)]
    [TestCase("bitxor", double.NaN, 1.0, 1.0)]
    [TestCase("bitxor", 2.0, double.NaN, 2.0)]
    [TestCase("bitxor", double.NaN, double.NaN, 0.0)]
    [TestCase("bitxor", double.NegativeInfinity, 1.0, 1.0)]
    [TestCase("bitxor", 2.0, double.NegativeInfinity, 2.0)]
    [TestCase("bitxor", double.NegativeInfinity, double.NegativeInfinity, 0.0)]
    [TestCase("lshift", 4.0, 0.0, 4.0)]
    [TestCase("lshift", 4.0, double.NaN, 4.0)]
    [TestCase("lshift", 4.0, double.PositiveInfinity, 4.0)]
    [TestCase("lshift", 4.0, double.NegativeInfinity, 4.0)]
    [TestCase("lshift", 4.0, 1.0, 8.0)]
    [TestCase("lshift", 4.0, -1.0, 0.0)]
    [TestCase("lshift", 4.0, -2.0, 0.0)]
    [TestCase("lshift", 4.0, -3.0, 4 << 29)]
    [TestCase("lshift", 0x40000000, 1.0, unchecked((int)0x80000000U))]
    [TestCase("lshift", 0x40000000, 2.0, 0.0)]
    [TestCase("rshift", 4.0, 0.0, 4.0)]
    [TestCase("rshift", 4.0, double.NaN, 4.0)]
    [TestCase("rshift", 4.0, double.PositiveInfinity, 4.0)]
    [TestCase("rshift", 4.0, double.NegativeInfinity, 4.0)]
    [TestCase("rshift", 4.0, 1.0, 2.0)]
    [TestCase("rshift", 4.0, -1.0, 0.0)]
    [TestCase("rshift", 4.0, -2.0, 0.0)]
    [TestCase("rshift", 4.0, -31.0, 2.0)]
    [TestCase("rshift", 2.0, 1.0, 1.0)]
    [TestCase("rshift", 2.0, 2.0, 0.0)]
    public void BitwiseOperatorsCanBeEvaluated(string op, double a, double b, double expected)
    {
        var binOp = Node.BinOp(op, Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        result.Should().Be(expected.ToValue());
    }

    [TestCase("and", "0", "false", false)]
    [TestCase("and", "0", "true", false)]
    [TestCase("and", "nan", "0.0", false)]
    [TestCase("and", "nan", "inf", true)]
    [TestCase("or", "0x0", "0b0", false)]
    [TestCase("or", "0o0", "1.0", true)]
    [TestCase("or", "nan", "0.0", true)]
    [TestCase("or", "-inf", "inf", true)]
    public void LogicalOperatorCanBeEvaluated(string op, string a, string b, bool expected)
    {
        var binOp = Node.BinOp(op, Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        result.Should().Be(expected.ToValue());
    }

    [TestCase("false", "0.0", "false")]
    [TestCase("0", "0.0", "0")]
    [TestCase("true", "0.0", "0.0")]
    [TestCase("1", "0x0", "0x0")]
    [TestCase("true", "1.0", "1.0")]
    [TestCase("true", "true", "true")]
    public void AndThenReturnsTheFirstFalseValueOrTheLastTrueValue(string a, string b, string expected)
    {
        var binOp = Node.BinOp("andthen", Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        result.Should().Be(expected.ToValue());
    }

    [Test]
    public void AndThensSecondArgumentIsNotEvaluatedIfTheFirstArgumentEvaluatesToFalse()
    {
        var evaluated = false;
        Environment.GlobalScope.DefineCommand("a", new SimpleCommand((args) => BoolValue.False));
        Environment.GlobalScope.DefineCommand("b", new SimpleCommand((args) => {
            evaluated = true; return BoolValue.True;
        }));
        var binOp = Node.BinOp(
            "andthen",
            Node.Command(Node.Literal("a"), new ListValue()),
            Node.Command(Node.Literal("b"), new ListValue()));

        var result = Evaluate(binOp);

        evaluated.Should().BeFalse();
    }

    [TestCase("true", "1.0", "true")]
    [TestCase("1", "1.0", "1")]
    [TestCase("false", "1.0", "1.0")]
    [TestCase("false", "1x0", "1x0")]
    [TestCase("false", "0.0", "0.0")]
    [TestCase("false", "false", "false")]
    public void OrElseReturnsTheFirstTrueValueOrTheLastFalseValue(string a, string b, string expected)
    {
        var binOp = Node.BinOp("orelse", Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        result.Should().Be(expected.ToValue());
    }

    [Test]
    public void OrElsesSecondArgumentIsNotEvaluatedIfTheFirstArgumentEvaluatesToTrue()
    {
        var evaluated = false;
        Environment.GlobalScope.DefineCommand("a", new SimpleCommand((args) => BoolValue.True));
        Environment.GlobalScope.DefineCommand("b", new SimpleCommand((args) => { evaluated = true; return BoolValue.False; }));
        var binOp = Node.BinOp(
            "orelse",
            Node.Command(Node.Literal("a"), new ListValue()),
            Node.Command(Node.Literal("b"), new ListValue()));

        var result = Evaluate(binOp);

        evaluated.Should().BeFalse();
    }

    [TestCase("unknown")]
    [TestCase("foo")]
    public void EvaluatingAnUnknownBinaryOperatorThrowsAValueError(string op)
    {
        var binOp = Node.BinOp(op, Node.Literal("a"), Node.Literal("b"));

        Evaluator.Invoking(e => e.Evaluate(Environment, binOp))
            .Should().Throw<ValueError>().WithMessage("Invalid binary operator.");
    }

    [TestCase(1.0, 0.0, 1)]
    [TestCase(-1.0, 0.0, -1)]
    [TestCase(1.0, -0.0, -1)]
    [TestCase(-1.0, -0.0, 1)]
    public void DivisionsByZeroEvaluateToInfinityWithMatchingSign(double a, double zero, int sign)
    {
        var binOp = Node.BinOp("div", Node.Literal(a), Node.Literal(zero));

        var result = Evaluate(binOp).ToDouble();

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
    public void IfAnyOfTheOperandsOfAnArithmeticOperatorAreNaNTheResultIsNaN(string op, double a, double b)
    {
        var binOp = Node.BinOp(op, Node.Literal(a), Node.Literal(b));

        var result = Evaluate(binOp);

        double.IsNaN(result.ToDouble()).Should().BeTrue();
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new BinOpEvaluator();
    }
}