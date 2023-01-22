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

        node.Should().Be(NodeBuilder.Shared.Script(
            NodeBuilder.Shared.Command(NodeBuilder.Shared.Literal("print".ToValue()),
            new ListValue(
                NodeBuilder.Shared.Literal("jello,".ToValue()),
                NodeBuilder.Shared.Literal("world".ToValue())
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

        node.Should().Be(NodeBuilder.Shared.Script(
            NodeBuilder.Shared.Command(NodeBuilder.Shared.Literal("print".ToValue()),
            new ListValue(
                NodeBuilder.Shared.Literal("jello,".ToValue()),
                NodeBuilder.Shared.Literal("world".ToValue())
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

        node.Should().Be(NodeBuilder.Shared.Script());
    }

    [Test]
    public void MultipleCommandsCanBeParsedSeparatedByRunsOfCommandSeparatorsAndWordSeparators()
    {
        var parser = new ScriptParser();
        var source = "print one;print two ;; print three";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(NodeBuilder.Shared.Script(
            NodeBuilder.Shared.Command(NodeBuilder.Shared.Literal("print".ToValue()),
            new ListValue(
                NodeBuilder.Shared.Literal("one".ToValue())
            )),
            NodeBuilder.Shared.Command(NodeBuilder.Shared.Literal("print".ToValue()),
            new ListValue(
                NodeBuilder.Shared.Literal("two".ToValue())
            )),
            NodeBuilder.Shared.Command(NodeBuilder.Shared.Literal("print".ToValue()),
            new ListValue(
                NodeBuilder.Shared.Literal("three".ToValue())
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

        node.Should().Be(NodeBuilder.Shared.Script(
            NodeBuilder.Shared.Command(NodeBuilder.Shared.Literal("say".ToValue()),
            new ListValue(
                NodeBuilder.Shared.Literal("hi!".ToValue())
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
    public void WhenConfiguredAsASubscriptParserIfTheParserReachesTheEndOfTheSourceBeforeFindingAScriptEndCharacterAErrorIsThrown()
    {
        var parser = new ScriptParser(true);
        var source = "{say hi!";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-file.");
    }
}