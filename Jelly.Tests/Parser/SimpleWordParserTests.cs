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
        var scanner = new Scanner("hello");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        node.Should().Be(Node.Literal("hello".ToValue()));
    }

    [Test]
    public void ALiteralNodeIsParsedFromSourceFromAGivenPosition()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("hello, goodbye", 7);
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        node.Should().Be(Node.Literal("goodbye".ToValue()));
    }

    [Test]
    public void ThePositionIsAdvanceToTheEndOfTheParsedWord()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("hello, goodbye", 7);
        
        parser.Parse(scanner, TestParserConfig.Shared);

        scanner.Position.Should().Be(14);
    }

    [Test]
    public void TheParsingOfAWordEndsAtAWordSeparator()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("hello, goodbye");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        node.Should().Be(Node.Literal("hello,".ToValue()));
        scanner.Position.Should().Be(6);
    }

    [Test]
    public void IfTheSourceContainsNoWordCharactersNoLiteralNodeIsReturnedAndThePositionIsNotAdvanced()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("    ", 2);
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        node.Should().BeNull();
        scanner.Position.Should().Be(2);
    }

    [Test]
    public void SpecialCharacterThatWouldNormalStopWordParsingAreIncludedInTheWordIfProceededWithAEscapeCharacter()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner(@"\ \\");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        node.Should().Be(Node.Literal(@" \".ToValue()));
    }

    [Test]
    public void IfAEscapeCharacterIsAtTheEndOfTheSourceAParseErrorIsThrown()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner(@"hi\");
        
        parser.Invoking(p => p.Parse(scanner, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after escape-character.");
    }

    [Test]
    public void IfAOperatorIsEncounteredTheSimpleWordEnds()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("E=mc2");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        node.Should().Be(Node.Literal("E".ToValue()));
        scanner.Position.Should().Be(1);
    }
}