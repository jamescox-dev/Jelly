namespace Jelly.Tests.Parser;

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
        node.Should().Be(Node.Script(0, 18,
            Node.Command(0, 18, Node.Literal(0, 5, "print"), new ListValue(
                Node.Literal(6, 12, "jello,"),
                Node.Literal(13, 18, "world")
            ))
        ));
    }

    [Test]
    public void TheScriptParserSkipsRunsOfWordSeparatorsAndCommandSeparatorsBeforeScanningACommand()
    {
        var parser = new ScriptParser();
        var scanner = new Scanner(" ; ; ;  print jello, world");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Script(0, 26,
            Node.Command(8, 26, Node.Literal(8, 13, "print"), new ListValue(
                Node.Literal(14, 20, "jello,"),
                Node.Literal(21, 26, "world")
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

        node.Should().Be(Node.Script(0, source.Length));
    }

    [Test]
    public void MultipleCommandsCanBeParsedSeparatedByRunsOfCommandSeparatorsAndWordSeparators()
    {
        var parser = new ScriptParser();
        var scanner = new Scanner("print one;print two ;; print three");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Script(0, 34,
            Node.Command(0, 9, Node.Literal(0, 5, "print"), new ListValue(
                Node.Literal(6, 9, "one")
            )),
            Node.Command(10, 19, Node.Literal(10, 15, "print"), new ListValue(
                Node.Literal(16, 19, "two")
            )),
            Node.Command(23, 34, Node.Literal(23, 28, "print"), new ListValue(
                Node.Literal(29, 34, "three")
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
        node.Should().Be(Node.Script(0, 8,
            Node.Command(1, 7, Node.Literal(1, 4, "say"), new ListValue(
                Node.Literal(5, 7, "hi")
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

        node.Should().Be(Node.Script(0, 8,
            Node.Command(0, 8, Node.Literal(0, 3, "say"), new ListValue(
                Node.Literal(4, 8, "hi!}")
            ))
        ));
    }

    [Test]
    public void WhenConfiguredAsASubscriptParserIfTheParserReachesTheEndOfTheSourceBeforeFindingAScriptEndCharacterAErrorIsThrown()
    {
        var parser = new ScriptParser(true);
        var scanner = new Scanner("{say hi!");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<MissingEndTokenError>().WithMessage("Unexpected end-of-file.")
            .Where(e => e.StartPosition == 8 && e.EndPosition == 8);
    }
}