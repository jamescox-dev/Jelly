namespace Jelly.Parser.Tests;

using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class SimpleWordParserTests
{
    [Test]
    public void ALiteralNodeIsParsedFromSource()
    {
        var parser = new SimpleWordParser();
        var source = "hello";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Literal("hello".ToValue()));
    }

    [Test]
    public void ALiteralNodeIsParsedFromSourceFromAGivenPosition()
    {
        var parser = new SimpleWordParser();
        var source = "hello, goodbye";
        var position = 7;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Literal("goodbye".ToValue()));
    }

    [Test]
    public void ThePositionOfIsAdvanceToTheEndOfTheParsedWord()
    {
        var parser = new SimpleWordParser();
        var source = "hello, goodbye";
        var position = 7;
        
        parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(14);
    }

    [Test]
    public void TheParsingOfAWordEndsAtAWordSeparator()
    {
        var parser = new SimpleWordParser();
        var source = "hello, goodbye";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Literal("hello,".ToValue()));
        position.Should().Be(6);
    }

    [Test]
    public void IfTheSourceContainsNoWordCharactersNoLiteralNodeIsReturnedAndThePositionIsNotAdvanced()
    {
        var parser = new SimpleWordParser();
        var source = "    ";
        var position = 2;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().BeNull();
        position.Should().Be(2);
    }

    [Test]
    public void SpecialCharacterThatWouldNormalStopWordParsingAreIncludedInTheWordIfProceededWithAEscapeCharacter()
    {
        var parser = new SimpleWordParser();
        var source = @"\ \\";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Literal(@" \".ToValue()));
    }

    [Test]
    public void IfAEscapeCharacterIsAtTheEndOfTheSourceAParseErrorIsThrown()
    {
        var parser = new SimpleWordParser();
        var source = @"hi\";
        var position = 0;
        
        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after escape-character '\\'.");
    }

    [Test]
    public void IfAOperatorIsEncounteredTheSimpleWordEnds()
    {
        var parser = new SimpleWordParser();
        var source = "E=mc2";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Literal("E".ToValue()));
        position.Should().Be(1);
    }
}