namespace Jelly.Parser.Tests;

[TestFixture]
public class VariableParserTests
{
    [Test]
    public void WhenTheCharacterAtTheCurrentPositionIsNotAVariableCharacterNotVariableNodeIsParsed()
    {
        var parser = new VariableParser(new Mock<IParser>().Object);
        var scanner = new Scanner("pi");

        var node = parser.Parse(scanner);

        node.Should().BeNull();
    }

    [Test]
    public void AVariableParserCanHaveAnOptionalTerminatingCharacter()
    {
        var parser = new VariableParser(new Mock<IParser>().Object, '>');
        var scanner = new Scanner("$pi>are>squared");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable("pi", 0, 3));
    }

    [Test]
    public void ASimpleWordParserCanBeConfiguredToTerminateAtAOperator()
    {
        var parser = new VariableParser(new Mock<IParser>().Object, terminateAtOperator: true);
        var scanner = new Scanner("$pi++squared");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable("pi", 0, 3));
    }

    [Test]
    public void AVariableNodeIsParsedWhenTheCurrentCharacterIsAVariableCharacter()
    {
        var parser = new VariableParser(new Mock<IParser>().Object);
        var scanner = new Scanner("$pi");

        var node = parser.Parse(scanner);

        node.Should().NotBeNull();
    }

    [Test]
    public void TheNameOfTheVariableIsAllTheCharactersFollowingTheVariableCharacterUntilASpecialCharacterIsEncountered()
    {
        var parser = new VariableParser(new Mock<IParser>().Object);
        var scanner = new Scanner("$foo ");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable("foo", 0, 4));
    }

    [Test]
    public void IfANameOfZeroCharactersInLengthIsEncounteredAnErrorIsThrown()
    {
        var parser = new VariableParser(new Mock<IParser>().Object);
        var scanner = new Scanner("$$");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("A variable must have a name.")
            .Where(e => e.StartPosition == 0 && e.EndPosition == 1);
    }

    [Test]
    public void AExpressionImmediatelyFollowingTheVariableNameIsInterpretedAsAListIndexer()
    {
        var expressionParser = new Mock<IParser>();
        expressionParser.Setup(m => m.Parse(It.IsAny<Scanner>())).Returns((Scanner s) => {
            s.Advance(3);
            return Node.Literal(1);
        });
        var parser = new VariableParser(expressionParser.Object);
        var scanner = new Scanner("$foo(1)");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable(0, 7, "foo", Node.ListIndexer(4, 7, Node.Literal(1))));
    }

    [Test]
    public void ASeriesOfListIndexerCanBeGiven()
    {
        var count = 0;
        var expressionParser = new Mock<IParser>();
        expressionParser.Setup(m => m.Parse(It.IsAny<Scanner>())).Returns((Scanner s) => {
            s.Advance(3);
            return Node.Literal(++count);
        });
        var parser = new VariableParser(expressionParser.Object);
        var scanner = new Scanner("$foo(1)(2)(3)");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable(0, 13, "foo",
            Node.ListIndexer(4, 7, Node.Literal(1)),
            Node.ListIndexer(7, 10, Node.Literal(2)),
            Node.ListIndexer(10, 13, Node.Literal(3))));
    }

    [Test]
    public void ADictIndexerCharacterImmediatelyFollowingTheVariableNameImmediatelyFollowedByAnExpressionIsInterpretedAsADictIndexer()
    {
        var expressionParser = new Mock<IParser>();
        expressionParser.Setup(m => m.Parse(It.IsAny<Scanner>())).Returns((Scanner s) => {
            s.Advance(5);
            return Node.Literal("a");
        });
        var parser = new VariableParser(expressionParser.Object);
        var scanner = new Scanner("$bar.('a')");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable(0, 10, "bar", Node.DictIndexer(4, 10, Node.Literal("a"))));
    }

    [Test]
    public void ADictIndexerCharacterImmediatelyFollowingTheVariableNameButNotImmediatelyFollowedByAnExpressionThrowsAnException()
    {
        var expressionParser = new ExpressionParser();
        var parser = new VariableParser(expressionParser);
        var scanner = new Scanner("$bar. ('a')");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("dict indexer missing key expression.")
            .Where(e => e.StartPosition == 4 && e.EndPosition == 5);
    }

    [Test]
    public void ASeriesOfListAndDictIndexerCanBeGiven()
    {
        var count = 0;
        var expressionParser = new Mock<IParser>();
        expressionParser.Setup(m => m.Parse(It.IsAny<Scanner>())).Returns((Scanner s) => {
            s.Advance(3);
            return Node.Literal(++count);
        });
        var parser = new VariableParser(expressionParser.Object);
        var scanner = new Scanner("$foo(1).(2)(3).(4)");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable(0, 18, "foo",
            Node.ListIndexer(4, 7, Node.Literal(1)),
            Node.DictIndexer(7, 11, Node.Literal(2)),
            Node.ListIndexer(11, 14, Node.Literal(3)),
            Node.DictIndexer(14, 18, Node.Literal(4))));
    }
}