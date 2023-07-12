

namespace Jelly.Parser.Tests;

[TestFixture]
public class CommentParserTests
{
    [Test]
    public void NoNodeIsEverReturnedByTheParser()
    {
        var parser = new CommentParser();
        var scanner = new Scanner("#comment");

        var node = parser.Parse(scanner);

        node.Should().BeNull();
    }

    [TestCase("#comment", 8)]
    [TestCase("#another comment\n then something else", 16)]
    public void IfACommentBeginCharacterIsEncounteredThePositionIsAdvancedToTheNextCommentEndOrEndOfInput(string source, int expectedPosition)
    {
        var parser = new CommentParser();
        var scanner = new Scanner(source);

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(expectedPosition);
    }

    [Test]
    public void IfACommentBeginCharacterIsNotAtTheCurrentPositionThePositionIsNotAltered()
    {
        var parser = new CommentParser();
        var scanner = new Scanner("not a comment");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(0);
    }
}