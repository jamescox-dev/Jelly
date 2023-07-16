namespace Jelly.Parser.Tests;

[TestFixture]
public class ScriptParserTests
{
    [Test]
    public void AScriptParsersACommandFromSource()
    {
        var parser = new ScriptParser();
        var scanner = new Scanner("print jello, world");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(18);
        node.Should().Be(Node.Script(
            Node.Command(Node.Literal("print", 0, 5),
            new ListValue(
                Node.Literal("jello,", 6, 12),
                Node.Literal("world", 13, 18)
            ))
        ));
    }

    [Test]
    public void TheScriptParserSkipsRunsOfWordSeparatorsAndCommandSeparatorsBeforeScanningACommand()
    {
        var parser = new ScriptParser();
        var scanner = new Scanner(" ; ; ;  print jello, world");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Script(
            Node.Command(Node.Literal("print", 8, 13),
            new ListValue(
                Node.Literal("jello,", 14, 20),
                Node.Literal("world", 21, 26)
            ))
        ));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(";;;")]
    [TestCase("; ; ; ")]
    public void WhenTheSourceIsEmptyOrJustACollectionOfWordAndCommandSeparatorsAScriptWithNoCommandsIsReturned(string source)
    {
        var parser = new ScriptParser();
        var scanner = new Scanner(source);

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Script());
    }

    [Test]
    public void MultipleCommandsCanBeParsedSeparatedByRunsOfCommandSeparatorsAndWordSeparators()
    {
        var parser = new ScriptParser();
        var scanner = new Scanner("print one;print two ;; print three");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Script(
            Node.Command(Node.Literal("print", 0, 5),
            new ListValue(
                Node.Literal("one", 6, 9)
            )),
            Node.Command(Node.Literal("print", 10, 15),
            new ListValue(
                Node.Literal("two", 16, 19)
            )),
            Node.Command(Node.Literal("print", 23, 28),
            new ListValue(
                Node.Literal("three", 29, 34)
            ))
        ));
    }

    [Test]
    public void WhenConfiguredAsASubscriptParserTheScriptMustBeSurroundedByScriptAndScriptEndCharacters()
    {
        var parser = new ScriptParser(true);
        var scanner = new Scanner("{say hi}");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(8);
        node.Should().Be(Node.Script(
            Node.Command(Node.Literal("say", 1, 4),
            new ListValue(
                Node.Literal("hi", 5, 7)
            ))
        ));
    }

    [Test]
    public void WhenConfiguredAsASubscriptParserIfTheScriptDoesNotStartWithAScriptCharacterNoScriptNodeIsReturned()
    {
        var parser = new ScriptParser(true);
        var scanner = new Scanner("say hi!}");

        var node = parser.Parse(scanner);

        node.Should().BeNull();
    }

    [Test]
    public void WhenNotConfiguredAsASubscriptParserIfAScriptEndCharacterIsEncounteredItIsTreatedAsARegularCharacter()
    {
        var parser = new ScriptParser();
        var scanner = new Scanner("say hi!}");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Script(
            Node.Command(Node.Literal("say", 0, 3),
            new ListValue(
                Node.Literal("hi!}", 4, 8)
            ))
        ));
    }

    [Test]
    public void WhenConfiguredAsASubscriptParserIfTheParserReachesTheEndOfTheSourceBeforeFindingAScriptEndCharacterAErrorIsThrown()
    {
        var parser = new ScriptParser(true);
        var scanner = new Scanner("{say hi!");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<MissingEndTokenError>().WithMessage("Unexpected end-of-file.");
    }
}