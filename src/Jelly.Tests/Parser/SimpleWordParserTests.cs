namespace Jelly.Tests.Parser;

[TestFixture]
public class SimpleWordParserTests
{
    [Test]
    public void ALiteralNodeIsParsedFromSource()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("hello");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 5, "hello".ToValue()));
    }

    [Test]
    public void ASimpleWordParserCanHaveAnOptionalTerminatingCharacter()
    {
        var parser = new SimpleWordParser('>');
        var scanner = new Scanner("hello>world");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 5, "hello".ToValue()));
    }

    [Test]
    public void ASimpleWordParserCanBeConfiguredToTerminateAtAOperator()
    {
        var parser = new SimpleWordParser(terminateAtOperator: true);
        var scanner = new Scanner("hello<=world");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 5, "hello".ToValue()));
    }

    [Test]
    public void ALiteralNodeIsParsedFromSourceFromAGivenPosition()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("hello, goodbye", 7);

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(7, 14, "goodbye".ToValue()));
    }

    [Test]
    public void ThePositionIsAdvanceToTheEndOfTheParsedWord()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("hello, goodbye", 7);

        parser.Parse(scanner);

        scanner.Position.Should().Be(14);
    }

    [Test]
    public void TheParsingOfAWordEndsAtAWordSeparator()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("hello, goodbye");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 6, "hello,".ToValue()));
        scanner.Position.Should().Be(6);
    }

    [Test]
    public void IfTheSourceContainsNoWordCharactersNoLiteralNodeIsReturnedAndThePositionIsNotAdvanced()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("    ", 2);

        var node = parser.Parse(scanner);

        node.Should().BeNull();
        scanner.Position.Should().Be(2);
    }

    [Test]
    public void SpecialCharacterThatWouldNormallyStopWordParsingAreIncludedInTheWordIfProceededWithAEscapeCharacter()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner(@"\ \\");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 4, @" \".ToValue()));
    }

    [Test]
    public void IfAEscapeCharacterIsAtTheEndOfTheSourceAParseErrorIsThrown()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner(@"hi\");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after escape-character.");
    }

    [Test]
    public void ParsingStopsAtAnAssignmentOperator()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("hi=");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 2, "hi".ToValue()));
        scanner.Position.Should().Be(2);
    }

    [Test]
    public void IfParsingStartsAtAnAssignmentOperatorItIsReturned()
    {
        var parser = new SimpleWordParser();
        var scanner = new Scanner("hi=", 2);

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(2, 3, "=".ToValue()));
        scanner.Position.Should().Be(3);
    }
}