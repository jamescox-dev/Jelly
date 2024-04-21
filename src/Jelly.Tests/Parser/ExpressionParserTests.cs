namespace Jelly.Tests.Parser;

[TestFixture]
public class ExpressionParserTests
{
    ExpressionParser _parser = null!;

    [Test]
    public void AnEmptyExpressionHasNoSubExpressions()
    {
        var scanner = new Scanner("()");

        var expr = _parser.Parse(scanner);

        scanner.Position.Should().Be(2);
        expr.Should().Be(Node.Expression(0, 2));
    }

    [Test]
    public void ACommaIntroducesANewSubExpression()
    {
        var scanner = new Scanner("(,)");

        var expr = _parser.Parse(scanner);

        scanner.Position.Should().Be(3);
        expr.Should().Be(Node.Expression(0, 3, Node.Literal(1, 1, Value.Empty), Node.Literal(2, 2, Value.Empty)));
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
            .WithMessage("Unexpected end-of-file.")
            .Where(e => e.StartPosition == 1 && e.EndPosition == 1);
    }

    [Test]
    public void LiteralWordsThatCanBeParsedAsNumbersAreReturnedNumbers()
    {
        var scanner = new Scanner("(56.5)");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(Node.Expression(0, 6, Node.Literal(1, 5, 56.5)));
    }

    [Test]
    public void LeadingAndTrailSpacesAroundWordsAreIgnored()
    {
        var scanner = new Scanner("(\t\n\r 12 \n\r\t)");

        var expr = _parser.Parse(scanner);

        scanner.Position.Should().Be(12);
        expr.Should().Be(Node.Expression(0, 12, Node.Literal(5, 7, 12.0)));
    }

    [TestCase("+", "add", 5)]
    [TestCase("*", "mul", 5)]
    [TestCase("<", "lt", 5)]
    [TestCase("<=", "lte", 6)]
    [TestCase("ne", "strne", 6)]
    [TestCase("eq", "streq", 6)]
    public void LiteralWordsThatCanBeInterpretedAsBinaryOperatorsReturnTheCorrectorOperator(string op, string expectedOp, int secondOperandPosition)
    {
        var scanner = new Scanner($"(1 {op} 2)");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(
            Node.Expression(0, secondOperandPosition + 2,
                Node.BinOp(1, secondOperandPosition + 1,
                    expectedOp,
                    Node.Literal(1, 2, 1.0),
                    Node.Literal(secondOperandPosition, secondOperandPosition + 1, 2.0))));
    }

    [TestCase("~", "bitnot", 3)]
    [TestCase("not", "not", 5)]
    [TestCase("+", "pos", 3)]
    [TestCase("-", "neg", 3)]
    public void LiteralWordsThatCanBeInterpretedAsUnaryOperatorsReturnTheCorrectorOperator(string op, string expectedOp, int operandPosition)
    {
        var scanner = new Scanner($"({op} 1)");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(
            Node.Expression(0, operandPosition + 2,
                Node.UniOp(1, operandPosition + 1,
                    expectedOp,
                    Node.Literal(operandPosition, operandPosition + 1, 1.0))));
    }

    [TestCase("(* 1)", 1, 2, "Unexpected operator in expression.")]
    [TestCase("(1 * * 1)", 5, 6, "Unexpected operator in expression.")]
    [TestCase("(1 10)", 3, 5, "Unexpected value in expression.")]
    [TestCase("(1 *)", 4, 4, "Missing value after operator in expression.")]
    public void InvalidExpressionsThrowParseErrors(string expression, int expectedStart, int expectedEnd, string expectedMessage)
    {
        var scanner = new Scanner(expression);

        _parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage(expectedMessage)
            .Where(e => e.StartPosition == expectedStart && e.EndPosition == expectedEnd);
    }

    [Test]
    public void OperandsCanBeScriptOrOtherExpressions()
    {
        var scanner = new Scanner("((1 + 2) * {avg 1 2 3})");

        var expr = _parser.Parse(scanner);

        expr.Should().Be(
            Node.Expression(0, 23,
                Node.BinOp(1, 22,
                    "mul",
                    Node.Expression(1, 8,
                        Node.BinOp(2, 7,
                            "add",
                            Node.Literal(2, 3, 1.0),
                            Node.Literal(6, 7, 2.0)
                        )
                    ),
                    Node.Script(11, 22,
                        Node.Command(12, 21, Node.Literal(12, 15, "avg"), new ListValue(
                            Node.Literal(16, 17, 1.0),
                            Node.Literal(18, 19, 2.0),
                            Node.Literal(20, 21, 3.0)
                        ))
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
            Node.Expression(0, 9,
                Node.BinOp(1, 8,
                    "add",
                    Node.Variable(1, 3, "a"),
                    Node.Variable(6, 8, "b")
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
            Node.Expression(0, 7,
                Node.BinOp(1, 6,
                    "add",
                    Node.Variable(1, 3, "a"),
                    Node.Variable(4, 6, "b")
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
            Node.Expression(0, 11,
                Node.BinOp(1, 10,
                    "add",
                    Node.BinOp(1, 6,
                        "mul",
                        Node.Literal(1, 2, 1.0),
                        Node.Literal(5, 6, 2.0)
                    ),
                    Node.Literal(9, 10, 3.0)
                )
            )
        );
        expr2.Should().Be(
            Node.Expression(0, 11,
                Node.BinOp(1, 10,
                    "add",
                    Node.Literal(1, 2, 1.0),
                    Node.BinOp(5, 10,
                        "mul",
                        Node.Literal(5, 6, 2.0),
                        Node.Literal(9, 10, 3.0)
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

        expr.Should().Be(Node.Expression(0, 13,
            Node.Command(
                Node.Literal(1, 4, "max"),
                new ListValue(
                    Node.Variable(5, 7, "a"),
                    Node.Variable(9, 11, "b")))));
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

        expr.Should().Be(Node.Expression(0, 7,
            Node.Command(
                Node.Literal(1, 4, "max"),
                new ListValue())));
    }

    [SetUp]
    public void Setup()
    {
        _parser = new ExpressionParser();
    }
}