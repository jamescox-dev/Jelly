namespace Jelly.Parser.Tests;

[TestFixture]
public class CommentParserTests
{
    [Test]
    public void NoNodeIsEverReturnedByTheParser()
    {
        var parser = new CommentParser();
        var source = "#comment";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().BeNull();
    }

    [TestCase("#comment", 8)]
    [TestCase("#another comment; then something else", 16)]
    public void IfACommentCharacterIsEncounteredThePositionIsAdvancedToTheNextCommandSeparatorOrEndOfInput(string source, int expectedPosition)
    {
        var parser = new CommentParser();
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(expectedPosition);
    }

    [Test]
    public void IfACommentCharacterIsNotAtTheCurrentPositionThePositionIsNotAltered()
    {
        var parser = new CommentParser();
        var source = "not a comment";
        var position = 0;
        
        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(position);
    }
}