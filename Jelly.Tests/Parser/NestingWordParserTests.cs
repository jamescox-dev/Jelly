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
        var source = "hello";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(0);
        node.Should().BeNull();
    }

    [Test]
    public void AEmptyLiteralNodeCanBeParsedWhenTheSourceIsAnOpenAnCloseNestingQuote()
    {
        var parser = new NestingWordParser();
        var source = "[]";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(2);
        node.Should().Be(Node.Literal(Value.Empty));
    }

    [Test]
    public void EverythingBetweenTheOpenAndCloseNestingQuotesIsIncludedInTheValueIncludingSpecialCharacters()
    {
        var parser = new NestingWordParser();
        var source = "[{hello}, $world!]";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(18);
        node.Should().Be(Node.Literal("{hello}, $world!".ToValue()));
    }

    [Test]
    public void OpenAndClosingNestingQuotesAreCountedWhenAMatchingNumberOfClosingBracketsIsEncounteredTheWordEnds()
    {
        var parser = new NestingWordParser();
        var source = "[open [ close ] close] not this ]";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(22);
        node.Should().Be(Node.Literal("open [ close ] close".ToValue()));
    }

    [Test]
    public void IfNotEnoughClosingNestingQuotesAreEncounteredAnErrorIsThrown()
    {
        var parser = new NestingWordParser();
        var source = "[this [never [ends]]...";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input in nesting-word.");
    }

    [Test]
    public void OpenAndClosingQuotesCanBeEscapedWithAnOddNumberOfEscapeCharactersAndAreThenNotCounted()
    {
        var parser = new NestingWordParser();
        var source = @"[ \[ \\\[ \\[ \\\\\] ]]";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(23);
        node.Should().Be(Node.Literal(@" \[ \\\[ \\[ \\\\\] ]".ToValue()));
    }
}