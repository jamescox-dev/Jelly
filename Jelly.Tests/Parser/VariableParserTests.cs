namespace Jelly.Parser.Tests;

using Jelly.Errors;

[TestFixture]
public class VariableParserTests
{
    [Test]
    public void WhenTheCharacterAtTheCurrentPositionIsNotAVariableCharacterNotVariableNodeIsParsed()
    {
        var parser = new VariableParser();
        var source = "pi";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().BeNull();
    }

    [Test]
    public void AVariableNodeIsParsedWhenTheCurrentCharacterIsAVariableCharacter()
    {
        var parser = new VariableParser();
        var source = "$pi";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().NotBeNull();
    }

    [Test]
    public void TheNameOfTheVariableIsAllTheCharactersFollowingTheVariableCharacterUntilASpecialCharacterIsEncountered()
    {
        var parser = new VariableParser();
        var source = "$pi ";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Variable("pi"));
    }

    [Test]
    public void IfANameOfZeroCharactersInLengthIsEncounteredAnErrorIsThrown()
    {
        var parser = new VariableParser();
        var source = "$$";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("A variable must have a name.");
    }

    [Test]
    public void IfAfterTheVariableCharacterAVariableDelimiterCharacterIsFoundTheVariableNameIsParsedUntilAVariableEndDelimiter()
    {
        var parser = new VariableParser();
        var source = "$[pi]";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(5);
        node.Should().Be(Node.Variable("pi"));
    }
}