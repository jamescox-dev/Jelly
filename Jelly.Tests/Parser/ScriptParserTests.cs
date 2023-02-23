namespace Jelly.Parser.Tests;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

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
            Node.Command(Node.Literal("print".ToValue()),
            new ListValue(
                Node.Literal("jello,".ToValue()),
                Node.Literal("world".ToValue())
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
            Node.Command(Node.Literal("print".ToValue()),
            new ListValue(
                Node.Literal("jello,".ToValue()),
                Node.Literal("world".ToValue())
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
            Node.Command(Node.Literal("print".ToValue()),
            new ListValue(
                Node.Literal("one".ToValue())
            )),
            Node.Command(Node.Literal("print".ToValue()),
            new ListValue(
                Node.Literal("two".ToValue())
            )),
            Node.Command(Node.Literal("print".ToValue()),
            new ListValue(
                Node.Literal("three".ToValue())
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
            Node.Command(Node.Literal("say".ToValue()),
            new ListValue(
                Node.Literal("hi".ToValue())
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
    public void WhenNotConfiguredAsASubscriptParserIfAScriptEndCharacterIsEncounteredAnErrorIsThrown()
    {
        var parser = new ScriptParser();
        var scanner = new Scanner("say hi!}");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected input '}'.");
    }

    [Test]
    public void WhenConfiguredAsASubscriptParserIfTheParserReachesTheEndOfTheSourceBeforeFindingAScriptEndCharacterAErrorIsThrown()
    {
        var parser = new ScriptParser(true);
        var scanner = new Scanner("{say hi!");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<MissingEndTokenError>().WithMessage("Unexpected end-of-file.");
    }

    [Test]
    public void IfAWordParserCanNotBeParsedAErrorIsThrown()
    {
        var parser = new ScriptParser();
        var scanner = new Scanner("]");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected input ']'.");
    }
}