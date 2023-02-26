namespace Jelly.Parser.Tests;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

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
        node.Should().Be(Node.Composite(Node.Literal("hi".ToValue())));
    }

    [Test]
    public void IfACharacterIsProceededWithAEscapeCharacterTheCharacterIsIncludedInTheValueButNotTheEscapeCharacter()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'\\\''");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(6);
        node.Should().Be(Node.Composite(Node.Literal(@"\'".ToValue())));
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

        node.Should().Be(Node.Composite(
            Node.Literal("hello, $name how do you do".ToValue())));
    }

    [Test]
    public void IfAScriptIsEncounteredInTheQuotedWordItIsIncludedInTheCompositeReturned()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner(@"'hello, {whoami} how do you do'");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Composite(
            Node.Literal("hello, ".ToValue()),
            Node.Script(Node.Command(Node.Literal("whoami".ToValue()), new ListValue())),
            Node.Literal(" how do you do".ToValue())));
    }

    [Test]
    public void IfAScriptIsEncounteredAndSubstitutionsAreNotEnabledItIsIncludedAsIsAndALiteralReturened()
    {
        var parser = new QuotedWordParser(false);
        var scanner = new Scanner(@"'hello, {whoami} how do you do'");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal("hello, {whoami} how do you do".ToValue()));
    }

    [Test]
    public void AQuoteFollowedByAQuoteReturnsAnEmptyComposite()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner("''");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Composite());
    }

    [Test]
    public void AQuotesWordMustBeginAndEndWithTheSameQuote()
    {
        var parser = new QuotedWordParser();
        var scanner = new Scanner("'hello\" world' not parsed!");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(14);
        node.Should().Be(Node.Composite(Node.Literal("hello\" world".ToValue())));
    }
}