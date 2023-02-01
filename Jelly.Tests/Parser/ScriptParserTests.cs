namespace Jelly.Parser.Tests;

using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class ScriptParserTests
{
    [Test]
    public void AScriptParsersACommandFromSource()
    {
        var parser = new ScriptParser();
        var source = "print jello, world";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(18);
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
        var source = " ; ; ;  print jello, world";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

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
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Script());
    }

    [Test]
    public void MultipleCommandsCanBeParsedSeparatedByRunsOfCommandSeparatorsAndWordSeparators()
    {
        var parser = new ScriptParser();
        var source = "print one;print two ;; print three";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

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
        var source = "{say hi!}";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(9);
        node.Should().Be(Node.Script(
            Node.Command(Node.Literal("say".ToValue()),
            new ListValue(
                Node.Literal("hi!".ToValue())
            ))
        ));
    }

    [Test]
    public void WhenConfiguredAsASubscriptParserIfTheScriptDoesNotStartWithAScriptCharacterNoScriptNodeIsReturned()
    {
        var parser = new ScriptParser(true);
        var source = "say hi!}";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().BeNull();
    }

    [Test]
    public void WhenNotConfiguredAsASubscriptParserIfAScriptEndCharacterIsEncounteredAnErrorIsThrown()
    {
        var parser = new ScriptParser();
        var source = "say hi!}";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected input '}'.");
    }

    [Test]
    public void WhenConfiguredAsASubscriptParserIfTheParserReachesTheEndOfTheSourceBeforeFindingAScriptEndCharacterAErrorIsThrown()
    {
        var parser = new ScriptParser(true);
        var source = "{say hi!";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-file.");
    }

    [Test]
    public void IfAWordParserCanNotBeParsedAErrorIsThrown()
    {
        var parser = new ScriptParser();
        var source = "]";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected input ']'.");
    }
}