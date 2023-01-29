namespace Jelly.Parser.Tests;

using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class QuotedWordParserTests
{
    [Test]
    public void IfTheSourceDoesNotStartWithAQuoteNoNodeIsParsed()
    {
        var parser = new QuotedWordParser();
        var source = "hi";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(0);
        node.Should().BeNull();
    }

    [Test]
    public void IfTheSourceStartsWithAQuoteACompositeIsParsedUntilTheNextQuote()
    {
        var parser = new QuotedWordParser();
        var source = "'hi'";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(4);
        node.Should().Be(Node.Composite(Node.Literal("hi".ToValue())));
    }

    [Test]
    public void IfACharacterIsProceededWithAEscapeCharacterTheCharacterIsIncludedInTheValueButNotTheEscapeCharacter()
    {
        var parser = new QuotedWordParser();
        var source = @"'\\\''";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(6);
        node.Should().Be(Node.Composite(Node.Literal(@"\'".ToValue())));
    }

    [Test]
    public void IfAEscapeCharacterTheCharacterIsFoundAtTheEndOfTheSourceAnErrorIsThrown()
    {
        var parser = new QuotedWordParser();
        var source = @"'\";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after escape-character '\\'.");
    }

    [Test]
    public void IfNoClosingQuoteIsFoundBeforeTheEndOfTheSourceAnErrorIsThrown()
    {
        var parser = new QuotedWordParser();
        var source = @"'this never ends!";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input in quoted-word.");
    }

    [Test]
    public void IfAVariableIsEncounteredInTheQuotedWordItIsNotIncludedInTheCompositeReturned()
    {
        var parser = new QuotedWordParser();
        var source = @"'hello, $name how do you do'";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Composite(
            Node.Literal("hello, $name how do you do".ToValue())));
    }

    [Test]
    public void IfAScriptIsEncounteredInTheQuotedWordItIsIncludedInTheCompositeReturned()
    {
        var parser = new QuotedWordParser();
        var source = @"'hello, {whoami} how do you do'";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Composite(
            Node.Literal("hello, ".ToValue()),
            Node.Script(Node.Command(Node.Literal("whoami".ToValue()), new ListValue())),
            Node.Literal(" how do you do".ToValue())));
    }

    [Test]
    public void AQuoteFollowedByAQuoteReturnsAnEmptyComposite()
    {
        var parser = new QuotedWordParser();
        var source = "''";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Composite());
    }
}