namespace Jelly.Parser.Tests;

using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class NestingWordParserTests
{
    [Test]
    public void NoNodeIsParsedIfTheSourceDoesNotBeginWithAnOpenNestingQuote()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("hello");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        scanner.Position.Should().Be(0);
        node.Should().BeNull();
    }

    [Test]
    public void AEmptyLiteralNodeCanBeParsedWhenTheSourceIsAnOpenAnCloseNestingQuote()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("[]");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        scanner.Position.Should().Be(2);
        node.Should().Be(Node.Literal(Value.Empty));
    }

    [Test]
    public void EverythingBetweenTheOpenAndCloseNestingQuotesIsIncludedInTheValueIncludingSpecialCharacters()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("[{hello}, $world!]");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        scanner.Position.Should().Be(18);
        node.Should().Be(Node.Literal("{hello}, $world!".ToValue()));
    }

    [Test]
    public void OpenAndClosingNestingQuotesAreCountedWhenAMatchingNumberOfClosingBracketsIsEncounteredTheWordEnds()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("[open [ close ] close] not this ]");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        scanner.Position.Should().Be(22);
        node.Should().Be(Node.Literal("open [ close ] close".ToValue()));
    }

    [Test]
    public void IfNotEnoughClosingNestingQuotesAreEncounteredAnErrorIsThrown()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("[this [never [ends]]...");
        
        parser.Invoking(p => p.Parse(scanner, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input in nesting-word.");
    }

    [Test]
    public void OpenAndClosingQuotesCanBeEscapedWithAnOddNumberOfEscapeCharactersAndAreThenNotCounted()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner(@"[ \[ \\\[ \\[ \\\\\] ]]");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        scanner.Position.Should().Be(23);
        node.Should().Be(Node.Literal(@" \[ \\\[ \\[ \\\\\] ]".ToValue()));
    }
}