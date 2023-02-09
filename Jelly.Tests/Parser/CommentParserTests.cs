namespace Jelly.Parser.Tests;

[TestFixture]
public class CommentParserTests
{
    [Test]
    public void NoNodeIsEverReturnedByTheParser()
    {
        var parser = new CommentParser();
        var scanner = new Scanner("#comment");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        node.Should().BeNull();
    }

    [TestCase("#comment", 8)]
    [TestCase("#another comment; then something else", 16)]
    public void IfACommentCharacterIsEncounteredThePositionIsAdvancedToTheNextCommandSeparatorOrEndOfInput(string source, int expectedPosition)
    {
        var parser = new CommentParser();
        var scanner = new Scanner(source);
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        scanner.Position.Should().Be(expectedPosition);
    }

    [Test]
    public void IfACommentCharacterIsNotAtTheCurrentPositionThePositionIsNotAltered()
    {
        var parser = new CommentParser();
        var scanner = new Scanner("not a comment");
        
        var node = parser.Parse(scanner, TestParserConfig.Shared);

        scanner.Position.Should().Be(0);
    }
}