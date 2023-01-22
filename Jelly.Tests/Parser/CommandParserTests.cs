namespace Jelly.Parser.Tests;

using Jelly.Parser;
using Jelly.Values;

[TestFixture]
public class CommandParserTests
{
    [Test]
    public void ACommandIsParsedFromSourceWhenItContainsOnlyOneWords()
    {
        var parser = new CommandParser();
        var source = "go";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(NodeBuilder.Shared.Command(
            NodeBuilder.Shared.Literal("go".ToValue()), 
            new ListValue()));
    }

    [Test]
    public void TheParserSkipsWordSeparatorsAndIncludeExtraWordsAsArguments()
    {
        var parser = new CommandParser();
        var source = "print hello, world";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(NodeBuilder.Shared.Command(
            NodeBuilder.Shared.Literal("print".ToValue()), 
            new ListValue(
                NodeBuilder.Shared.Literal("hello,".ToValue()), 
                NodeBuilder.Shared.Literal("world".ToValue()))));
    }

    [Test]
    public void ACommandIsNotParsedWhenTheSourceContainsNoWords()
    {
        var parser = new CommandParser();
        var source = "  ";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().BeNull();
    }
}