namespace Jelly.Parser.Tests;

using System.Text.Json;

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
        node.Should().Be(Node.Composite(0, 4, Node.Literal("hi", 1, 3)));
    }

    [Test]
    public void IfACharacterIsProceededWithAEscapeCharacterTheCharacterIsIncludedInTheValueButNotTheEscapeCharacter()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'\\\''");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(6);
        node.Should().Be(Node.Composite(0, 6, Node.Literal(@"\'", 1, 5)));
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
            .Throw<MissingEndTokenError>().WithMessage("Unexpected end-of-input in quoted-word.");
    }

    [Test]
    public void IfAVariableIsEncounteredInTheQuotedWordItIsNotIncludedInTheCompositeReturned()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'hello, $name how do you do'");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Composite(0, 28,
            Node.Literal("hello, $name how do you do", 1, 27)));
    }

    [Test]
    public void IfAScriptIsEncounteredInTheQuotedWordItIsIncludedInTheCompositeReturned()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'hello, {whoami} how do you do'");
        var expectedNode = Node.Composite(0, 31,
            Node.Literal("hello, ", 1, 8),
            Node.Script(Node.Command(Node.Literal("whoami", 9, 15), new ListValue(), 9, 15)),
            Node.Literal(" how do you do", 16, 30));

        var node = parser.Parse(scanner);

        node.Should().Be(expectedNode);
    }

    [Test]
    public void IfAScriptIsEncounteredAndSubstitutionsAreNotEnabledItIsIncludedAsIsAndALiteralReturned()
    {
        var parser = new QuotedWordParser(false);
        var scanner = new Scanner(@"'hello, {whoami} how do you do'");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal("hello, {whoami} how do you do", 0, 31));
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
    public void AQuotesWordMustBeginAndEndWithTheSameQuote()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner("'hello\" world' not parsed!");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(14);
        node.Should().Be(Node.Composite(0, 14, Node.Literal("hello\" world", 1, 13)));
    }
}