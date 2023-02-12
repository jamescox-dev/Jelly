namespace Jelly.Parser.Tests;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser;
using Jelly.Parser.Scanning;
using Jelly.Values;

[TestFixture]
public class CommandParserTests
{
    [Test]
    public void ACommandIsParsedFromSourceWhenItContainsOnlyOneWords()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("go");
        
        var node = parser.Parse(scanner);

        node.Should().Be(Node.Command(
            Node.Literal("go".ToValue()), 
            new ListValue()));
    }

    [Test]
    public void TheParserSkipsWordSeparatorsAndIncludeExtraWordsAsArguments()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("print hello, world");
        
        var node = parser.Parse(scanner);

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
        var scanner = new Scanner("  ");
        
        var node = parser.Parse(scanner);

        node.Should().BeNull();
    }

    [Test]
    public void WhenTheCommandNameIsAVariableNodeAndTheFirstArgumentIsAnEqualsOperatorAnAssignmentNodeIsReturned()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("$name =");
        
        var node = parser.Parse(scanner);

        node.Should().Be(Node.Assignment(
            "name", Node.Literal(Value.Empty)));
    }

    [Test]
    public void WhenTheCommandIsParsedAsAnAssignmentTheValueIsTheFirstNode()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("$name = Vic");
        
        var node = parser.Parse(scanner);

        node.Should().Be(Node.Assignment(
            "name", Node.Literal("Vic".ToValue())));
    }

    [Test]
    public void WhenAnAssignmentHasMoreThanOneAParseErrorIsThrown()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("$name = Vic & Bob");
        
        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected literal after assignment value.");
    }

    [Test]
    public void WhenTheCommandIsJustOneSingleVariableNodeThatIsReturned()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("$name");
        
        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable("name"));
    }

    [Test]
    public void WhenTheCommandIsJustOneExpressionNodeThatIsReturned()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("(a + b)");
        
        var node = parser.Parse(scanner);

        node.Should().Be(Node.Expression(
            Node.Literal("a".ToValue()), 
            Node.Literal("+".ToValue()), 
            Node.Literal("b".ToValue())));
    }
}