namespace Jelly.Parser.Tests;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;

[TestFixture]
public class VariableParserTests
{
    [Test]
    public void WhenTheCharacterAtTheCurrentPositionIsNotAVariableCharacterNotVariableNodeIsParsed()
    {
        var parser = new VariableParser();
        var scanner = new Scanner("pi");

        var node = parser.Parse(scanner);

        node.Should().BeNull();
    }

    [Test]
    public void AVariableNodeIsParsedWhenTheCurrentCharacterIsAVariableCharacter()
    {
        var parser = new VariableParser();
        var scanner = new Scanner("$pi");

        var node = parser.Parse(scanner);

        node.Should().NotBeNull();
    }

    [Test]
    public void TheNameOfTheVariableIsAllTheCharactersFollowingTheVariableCharacterUntilASpecialCharacterIsEncountered()
    {
        var parser = new VariableParser();
        var scanner = new Scanner("$pi ");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable("pi"));
    }

    [Test]
    public void IfANameOfZeroCharactersInLengthIsEncounteredAnErrorIsThrown()
    {
        var parser = new VariableParser();
        var scanner = new Scanner("$$");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("A variable must have a name.");
    }

    [Test]
    public void IfAfterTheVariableCharacterAVariableDelimiterCharacterIsFoundTheVariableNameIsParsedUntilAVariableEndDelimiter()
    {
        var parser = new VariableParser();
        var scanner = new Scanner("${pi}");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(5);
        node.Should().Be(Node.Variable("pi"));
    }

    [Test]
    public void IfAOperatorIsEncounteredTheVariableEnds()
    {
        var parser = new VariableParser();
        var scanner = new Scanner("$E=mc2");
        
        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable("E"));
        scanner.Position.Should().Be(2);
    }
}