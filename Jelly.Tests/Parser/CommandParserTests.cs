namespace Jelly.Parser.Tests;

using Jelly.Errors;
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

        node.Should().Be(Node.Command(
            Node.Literal("go".ToValue()), 
            new ListValue()));
    }

    [Test]
    public void TheParserSkipsWordSeparatorsAndIncludeExtraWordsAsArguments()
    {
        var parser = new CommandParser();
        var source = "print hello, world";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Command(
            Node.Literal("print".ToValue()), 
            new ListValue(
                Node.Literal("hello,".ToValue()), 
                Node.Literal("world".ToValue()))));
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

    [Test]
    public void WhenTheCommandNameIsAVariableNodeAndTheFirstArgumentIsAnEqualsOperatorAnAssignmentNodeIsReturned()
    {
        var parser = new CommandParser();
        var source = "$name =";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Assignment(
            "name", Node.Literal(Value.Empty)));
    }

    [Test]
    public void WhenTheCommandIsParsedAsAnAssignmentTheValueIsTheFirstNode()
    {
        var parser = new CommandParser();
        var source = "$name = Vic";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Assignment(
            "name", Node.Literal("Vic".ToValue())));
    }

    [Test]
    public void WhenAnAssignmentHasMoreThanOneAParseErrorIsThrown()
    {
        var parser = new CommandParser();
        var source = "$name = Vic & Bob";
        var position = 0;
        
        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected literal after assignment value.");
    }

    [Test]
    public void WhenTheCommandIsJustOneSingleVariableNodeThatIsReturned()
    {
        var parser = new CommandParser();
        var source = "$name";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Variable("name"));
    }
}