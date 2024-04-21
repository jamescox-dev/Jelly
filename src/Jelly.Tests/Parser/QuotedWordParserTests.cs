namespace Jelly.Tests.Parser;

[TestFixture]
public class QuotedWordParserTests
{
    [Test]
    public void IfTheSourceDoesNotStartWithAQuoteNoNodeIsParsed()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner("hi");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(0);
        node.Should().BeNull();
    }

    [Test]
    public void IfTheSourceStartsWithAQuoteACompositeIsParsedUntilTheNextQuote()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner("'hi'");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(4);
        node.Should().Be(Node.Composite(0, 4, Node.Literal(1, 3, "hi")));
    }

    [Test]
    public void IfACharacterIsProceededWithAEscapeCharacterTheCharacterIsIncludedInTheValueButNotTheEscapeCharacter()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'\\\''");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(6);
        node.Should().Be(Node.Composite(0, 6, Node.Literal(1, 5, @"\'")));
    }

    [Test]
    public void IfAEscapeCharacterIsFoundAtTheEndOfTheSourceAnErrorIsThrown()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'\");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after escape-character.");
    }

    [Test]
    public void IfNoClosingQuoteIsFoundBeforeTheEndOfTheSourceAnErrorIsThrown()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'this never ends!");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<MissingEndTokenError>().WithMessage("Unexpected end-of-input in quoted-word.")
            .Where(e => e.StartPosition == 17 && e.EndPosition == 17);
    }

    [Test]
    public void IfAVariableIsEncounteredInTheQuotedWordItIsNotIncludedInTheCompositeReturned()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'hello, $name how do you do'");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Composite(0, 28,
            Node.Literal(1, 27, "hello, $name how do you do")));
    }

    [Test]
    public void IfAScriptIsEncounteredInTheQuotedWordItIsIncludedInTheCompositeReturned()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'hello, {whoami} how do you do'");
        var expectedNode = Node.Composite(0, 31,
            Node.Literal(1, 8, "hello, "),
            Node.Script(8, 16, Node.Command(9, 15, Node.Literal(9, 15, "whoami"), new ListValue())),
            Node.Literal(16, 30, " how do you do"));

        var node = parser.Parse(scanner);

        node.Should().Be(expectedNode);
    }

    [Test]
    public void IfAScriptIsEncounteredAndSubstitutionsAreNotEnabledItIsIncludedAsIsAndALiteralReturned()
    {
        var parser = new QuotedWordParser(false);
        var scanner = new Scanner(@"'hello, {whoami} how do you do'");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 31, "hello, {whoami} how do you do"));
    }

    [Test]
    public void AQuoteFollowedByAQuoteReturnsAnEmptyComposite()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner("''");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Composite(0, 2));
    }

    [Test]
    public void AQuoteFollowedByAQuoteReturnsAnEmptyLiteralWhenSubstitutionsAreDisabled()
    {
        var parser = new QuotedWordParser(false);
        var scanner = new Scanner("''");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 2, ""));
    }

    [Test]
    public void AQuotesWordMustBeginAndEndWithTheSameQuote()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner("'hello\" world' not parsed!");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(14);
        node.Should().Be(Node.Composite(0, 14, Node.Literal(1, 13, "hello\" world")));
    }
}