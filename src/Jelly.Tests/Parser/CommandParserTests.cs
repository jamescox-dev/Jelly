namespace Jelly.Parser.Tests;

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
            Node.Literal("go", 0, 2),
            new ListValue()));
    }

    [Test]
    public void ACommandCanHaveOptionalTerminatingCharacter()
    {
        var parser = new CommandParser('>');
        var scanner = new Scanner("go>stop");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Command(
            Node.Literal("go", 0, 2),
            new ListValue()));
    }

    [Test]
    public void TheParserSkipsWordSeparatorsAndIncludeExtraWordsAsArguments()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("print hello, world");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Command(
            Node.Literal("print", 0, 5),
            new ListValue(
                Node.Literal("hello,", 6, 12),
                Node.Literal("world", 13, 18))));
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
            "name", Node.Literal("Vic", 8, 11)));
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
        var scanner = new Scanner("($a + $b)");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Expression(Node.BinOp("add", Node.Variable("a"), Node.Variable("b"))));
    }
}