namespace Jelly.Parser.Tests;

[TestFixture]
public class NestingWordParserTests
{
    [Test]
    public void NoNodeIsParsedIfTheSourceDoesNotBeginWithAnOpenNestingQuote()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("hello");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(0);
        node.Should().BeNull();
    }

    [Test]
    public void AEmptyLiteralNodeCanBeParsedWhenTheSourceIsAnOpenAnCloseNestingQuote()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("[]");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(2);
        node.Should().Be(Node.Literal(0, 2, Value.Empty));
    }

    [Test]
    public void EverythingBetweenTheOpenAndCloseNestingQuotesIsIncludedInTheValueIncludingSpecialCharacters()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("[{hello}, $world!]");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(18);
        node.Should().Be(Node.Literal(0, 18, "{hello}, $world!"));
    }

    [Test]
    public void OpenAndClosingNestingQuotesAreCountedWhenAMatchingNumberOfClosingBracketsIsEncounteredTheWordEnds()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("[open [ close ] close] not this ]");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(22);
        node.Should().Be(Node.Literal(0, 22, "open [ close ] close"));
    }

    [Test]
    public void IfNotEnoughClosingNestingQuotesAreEncounteredAnErrorIsThrown()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner("[this [never [ends]]...");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<MissingEndTokenError>().WithMessage("Unexpected end-of-input in nesting-word.")
            .Where(e => e.StartPosition == 23 && e.EndPosition == 23);
    }

    [Test]
    public void OpenAndClosingQuotesCanBeEscapedWithAnOddNumberOfEscapeCharactersAndAreThenNotCounted()
    {
        var parser = new NestingWordParser();
        var scanner = new Scanner(@"[ \[ \\\[ \\[ \\\\\] ]]");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(23);
        node.Should().Be(Node.Literal(0, 23, @" \[ \\\[ \\[ \\\\\] ]"));
    }
}