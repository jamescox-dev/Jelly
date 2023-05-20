namespace Jelly.Parser.Tests;

[TestFixture]
public class ExpressionParserTests
{
    ExpressionParser _parser = null!;

    [Test]
    public void AnEmptyExpressionHasNoSubexpressions()
    {
        var scanner = new Scanner("()");

        var expr = _parser.Parse(scanner);

        scanner.Position.Should().Be(2);
        expr.Should().Be(Node.Expression());
    }

    [Test]
    public void ACommaIntroducesANewSubexpression()
    {
        var scanner = new Scanner("(,)");

        var expr = _parser.Parse(scanner);

        scanner.Position.Should().Be(3);
        expr.Should().Be(Node.Expression(Node.Literal(Value.Empty), Node.Literal(Value.Empty)));
    }

    [Test]
    public void IfNoOpeningBracketStartsTheExpressionNotExpressionIsParsed()
    {
        var scanner = new Scanner("noway!");

        var expr = _parser.Parse(scanner);

        scanner.Position.Should().Be(0);
        expr.Should().BeNull();
    }

    [Test]
    public void IfNoClosingBracketEndsTheExpressionAParseErrorIsThrown()
    {
        var scanner = new Scanner("(");

        _parser.Invoking(p => p.Parse(scanner))
            .Should().Throw<MissingEndTokenError>()
            .WithMessage("Unexpected end-of-file.");
    }

    [Test]
    public void LiteralWordsThatCanBeParsedAsNumbersAreReturnedNumbers()
    {
        var scanner = new Scanner("(56.5)");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(Node.Expression(Node.Literal(56.5.ToValue())));
    }

    [Test]
    public void LeadingAndTrailSpacesAroundWordsAreIgnored()
    {
        var scanner = new Scanner("(\t\n\r 12 \n\r\t)");

        var expr = _parser.Parse(scanner);

        scanner.Position.Should().Be(12);
        expr.Should().Be(Node.Expression(Node.Literal(12.0.ToValue())));
    }

    [TestCase("+", "add")]
    [TestCase("*", "mul")]
    [TestCase("<", "lt")]
    [TestCase("<=", "lte")]
    public void LiteralWordsThatCanBeInterpretedAsBinaryOperatorsReturnTheCorrectorOperator(string op, string expectedOp)
    {
        var scanner = new Scanner($"(1 {op} 2)");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(
            Node.Expression(
                Node.BinOp(
                    expectedOp,
                    Node.Literal(1.0.ToValue()),
                    Node.Literal(2.0.ToValue()))));
    }

    [TestCase("~", "bitnot")]
    [TestCase("not", "not")]
    [TestCase("+", "pos")]
    [TestCase("-", "neg")]
    public void LiteralWordsThatCanBeInterpretedAsUnaryOperatorsReturnTheCorrectorOperator(string op, string expectedOp)
    {
        var scanner = new Scanner($"({op} 1)");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(
            Node.Expression(
                Node.UniOp(
                    expectedOp,
                    Node.Literal(1.0.ToValue()))));
    }

    [TestCase("(* 1)")]
    [TestCase("(1 * * 1)")]
    [TestCase("(1 1)")]
    [TestCase("(1 *)")]
    public void InvalidExpressionsThrowParseErrors(string expression)
    {
        var scanner = new Scanner(expression);

        _parser.Invoking(p => p.Parse(scanner)).Should().Throw<ParseError>("Invalid expression.");
    }

    [Test]
    public void OperandsCanBeScriptOrOtherExpressions()
    {
        var scanner = new Scanner("((1 + 2) * {avg 1 2 3})");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(
            Node.Expression(
                Node.BinOp(
                    "mul",
                    Node.Expression(
                        Node.BinOp(
                            "add",
                            Node.Literal(1.0.ToValue()),
                            Node.Literal(2.0.ToValue())
                        )
                    ),
                    Node.Script(
                        Node.Command(
                            Node.Literal("avg".ToValue()),
                            new ListValue(
                                Node.Literal(1.0.ToValue()),
                                Node.Literal(2.0.ToValue()),
                                Node.Literal(3.0.ToValue())
                            )
                        )
                    )
                )
            )
        );
    }

    [Test]
    public void OperandsCanBeVariables()
    {
        var scanner = new Scanner("($a + $b)");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(
            Node.Expression(
                Node.BinOp(
                    "add",
                    Node.Variable("a"),
                    Node.Variable("b")
                )
            )
        );
    }

    [Test]
    public void OperandsDoNotHaveToBeSeparatedBySapces()
    {
        var scanner = new Scanner("($a+$b)");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(
            Node.Expression(
                Node.BinOp(
                    "add",
                    Node.Variable("a"),
                    Node.Variable("b")
                )
            )
        );
    }

    [Test]
    public void OperatorsWithHigherPrecedenceAreReturnedLowerInTheParseTree()
    {
        var scanner1 = new Scanner($"(1 * 2 + 3)");
        var scanner2 = new Scanner($"(1 + 2 * 3)");

        var expr1 = _parser.Parse(scanner1);
        var expr2 = _parser.Parse(scanner2);

        expr1.Should().Be(
            Node.Expression(
                Node.BinOp(
                    "add",
                    Node.BinOp(
                        "mul",
                        Node.Literal(1.0.ToValue()),
                        Node.Literal(2.0.ToValue())
                    ),
                    Node.Literal(3.0.ToValue())
                )
            )
        );
        expr2.Should().Be(
            Node.Expression(
                Node.BinOp(
                    "add",
                    Node.Literal(1.0.ToValue()),
                    Node.BinOp(
                        "mul",
                        Node.Literal(2.0.ToValue()),
                        Node.Literal(3.0.ToValue())
                    )
                )
            )
        );
    }

    [Test]
    public void AFunctionCanBeParsedAndResultsInACommandBeingPutInItsPlace()
    {
        var scanner = new Scanner("(max($a, $b))");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(Node.Expression(
            Node.Command(
                Node.Literal("max"),
                new ListValue(
                    Node.Variable("a"),
                    Node.Variable("b")))));
    }

    [TestCase("(max)")]
    [TestCase("(max 1)")]
    public void IfTheFunctionNameIsNotFollowedByItsArgumentsAnErrorIsThrown(string expression)
    {
        var scanner = new Scanner(expression);

        _parser.Invoking(p => p.Parse(scanner)).Should().Throw<ParseError>("Invalid expression.");
    }

    [Test]
    public void IfTheFunctionTakesNoArgumentsNoArgumentsArePassedToTheGeneratedCommand()
    {
        var scanner = new Scanner("(max())");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(Node.Expression(
            Node.Command(
                Node.Literal("max"),
                new ListValue())));
    }

    [SetUp]
    public void Setup()
    {
        _parser = new ExpressionParser();
    }
}