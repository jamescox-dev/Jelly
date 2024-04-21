namespace Jelly.Tests.Evaluator;

[TestFixture]
public class UnaryOpEvaluatorTests : EvaluatorTestsBase
{
    [TestCase("notreal")]
    [TestCase("bla")]
    public void AnUnknownOperatorResultsInAValueErrorBeingThrown(string op)
    {
        var uniOp = Node.UniOp(op, Node.Literal(0));

        this.Invoking(e => e.Evaluate(uniOp))
            .Should().Throw<ValueError>().WithMessage("Invalid unary operator.");
    }

    [TestCase("foo", double.NaN)]
    [TestCase("inf", double.PositiveInfinity)]
    [TestCase("-inf", double.NegativeInfinity)]
    [TestCase("1.000_000", 1)]
    [TestCase("      0xff    ", 255)]
    [TestCase("      -1e3    ", -1000)]
    public void APositiveOperatorReturnsTheNumericValueOfItsOperand(string a, double expected)
    {
        var uniOp = Node.UniOp("pos", Node.Literal(a));

        var result = Evaluate(uniOp);

        result.Should().Be(expected.ToValue());
    }

    [TestCase("foo", double.NaN)]
    [TestCase("inf", double.NegativeInfinity)]
    [TestCase("-inf", double.PositiveInfinity)]
    [TestCase("1.000_000", -1)]
    [TestCase("      0xff    ", -255)]
    [TestCase("      -1e3    ", 1000)]
    public void ANegativeOperatorReturnsTheNegativeNumericValueOfItsOperand(string a, double expected)
    {
        var uniOp = Node.UniOp("neg", Node.Literal(a));

        var result = Evaluate(uniOp);

        result.Should().Be(expected.ToValue());
    }

    [TestCase("true", false)]
    [TestCase("false", true)]
    [TestCase("foo", false)]
    [TestCase("inf", false)]
    [TestCase("-inf", false)]
    [TestCase("1.000_000", false)]
    [TestCase("      0x0    ", true)]
    public void ALogicalNotCanBeEvaluated(string a, bool expected)
    {
        var uniOp = Node.UniOp("not", Node.Literal(a));

        var result = Evaluate(uniOp);

        result.Should().Be(expected.ToValue());
    }

    [TestCase("true", -2)]
    [TestCase("false", -1)]
    [TestCase("foo", -1)]
    [TestCase("inf", -1)]
    [TestCase("-inf", -1)]
    [TestCase("1.000_000", -2)]
    [TestCase("      0xffff    ", -65536)]
    public void ABitwiseNotCanBeEvaluated(string a, double expected)
    {
        var uniOp = Node.UniOp("bitnot", Node.Literal(a));

        var result = Evaluate(uniOp);

        result.Should().Be(expected.ToValue());
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new UnaryOpEvaluator();
    }
}