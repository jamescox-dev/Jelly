namespace Jelly.Parser.Tests;

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
        expr.Should().Be(Node.Expression(0, 3, Node.Literal(Value.Empty, 1, 1), Node.Literal(Value.Empty, 2, 2)));
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

        expr.Should().Be(Node.Expression(0, 6, Node.Literal(56.5, 1, 5)));
    }

    [Test]
    public void LeadingAndTrailSpacesAroundWordsAreIgnored()
    {
        var scanner = new Scanner("(\t\n\r 12 \n\r\t)");

        var expr = _parser.Parse(scanner);

        scanner.Position.Should().Be(12);
        expr.Should().Be(Node.Expression(0, 12, Node.Literal(12.0, 5, 7)));
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
                    Node.Literal(1.0, 1, 2),
                    Node.Literal(2.0, secondOperandPosition, secondOperandPosition + 1))));
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
                    Node.Literal(1.0, operandPosition, operandPosition + 1))));
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
            Node.Expression(0, 23,
                Node.BinOp(1, 22,
                    "mul",
                    Node.Expression(1, 8,
                        Node.BinOp(2, 7,
                            "add",
                            Node.Literal(1.0, 2, 3),
                            Node.Literal(2.0, 6, 7)
                        )
                    ),
                    Node.Script(11, 22,
                        Node.Command(
                            Node.Literal("avg", 12, 15),
                            new ListValue(
                                Node.Literal(1.0, 16, 17),
                                Node.Literal(2.0, 18, 19),
                                Node.Literal(3.0, 20, 21)
                            ),
                            12, 21
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
            Node.Expression(0, 9,
                Node.BinOp(1, 8,
                    "add",
                    Node.Variable("a", 1, 3),
                    Node.Variable("b", 6, 8)
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
                    Node.Variable("a", 1, 3),
                    Node.Variable("b", 4, 6)
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
                        Node.Literal(1.0, 1, 2),
                        Node.Literal(2.0, 5, 6)
                    ),
                    Node.Literal(3.0, 9, 10)
                )
            )
        );
        expr2.Should().Be(
            Node.Expression(0, 11,
                Node.BinOp(1, 10,
                    "add",
                    Node.Literal(1.0, 1, 2),
                    Node.BinOp(5, 10,
                        "mul",
                        Node.Literal(2.0, 5, 6),
                        Node.Literal(3.0, 9, 10)
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
                Node.Literal("max", 1, 4),
                new ListValue(
                    Node.Variable("a", 5, 7),
                    Node.Variable("b", 9, 11)))));
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
                Node.Literal("max", 1, 4),
                new ListValue())));
    }

    [SetUp]
    public void Setup()
    {
        _parser = new ExpressionParser();
    }
}