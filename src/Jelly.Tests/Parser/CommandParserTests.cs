namespace Jelly.Parser.Tests;

[TestFixture]
public class CommandParserTests
{
    [Test]
    public void ACommandIsParsedFromSourceWhenItContainsOnlyOneWord()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("go");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Command(0, 2, Node.Literal(0, 2, "go"), new ListValue()));
    }

    [Test]
    public void ACommandCanHaveOptionalTerminatingCharacter()
    {
        var parser = new CommandParser('>');
        var scanner = new Scanner("go>stop");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Command(0, 2, Node.Literal(0, 2, "go"), new ListValue()));
    }

    [Test]
    public void TheParserSkipsWordSeparatorsAndIncludeExtraWordsAsArguments()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("print hello, world");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Command(0, 18, Node.Literal(0, 5, "print"), new ListValue(
            Node.Literal(6, 12, "hello,"),
            Node.Literal(13, 18, "world"))));
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

        node.Should().Be(Node.Assignment(0, 7, "name", Node.Literal(7, 7, Value.Empty)));
    }

    [Test]
    public void WhenTheCommandIsParsedAsAnAssignmentTheValueIsTheFirstNode()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("$name = Vic");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Assignment(0, 11, "name", Node.Literal(8, 11, "Vic")));
    }

    [Test]
    public void WhenTheCommandIsParsedAsAnAssignmentAndTheVariableHasIndexersTheseIndexersAreUsedInTheAssignment()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("$test(1) = Bob");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Assignment(0, 14,
            "test",
            Node.Literal(11, 14, "Bob"),
            Node.ListIndexer(5, 8, Node.Expression(5, 8, Node.Literal(6, 7, "1")))));
    }

    [Test]
    public void WhenAnAssignmentHasMoreThanOneAParseErrorIsThrown()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("$name = Vic & Bob");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected literal after assignment value.")
            .Where(e => e.StartPosition == 12 && e.EndPosition == 17);
    }

    [Test]
    public void WhenTheCommandIsJustOneSingleVariableNodeThatIsReturned()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("$name");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable(0, 5, "name"));
    }

    [Test]
    public void WhenTheCommandIsJustOneExpressionNodeThatIsReturned()
    {
        var parser = new CommandParser();
        var scanner = new Scanner("($a + $b)");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Expression(0, 9, Node.BinOp(1, 8, "add", Node.Variable(1, 3, "a"), Node.Variable(6, 8, "b"))));
    }
}