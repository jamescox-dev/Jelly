using Jelly.Errors;

namespace Jelly.Parser.Tests;

[TestFixture]
public class EscapeCharacterParserTests
{
    [Test]
    public void NoCharactersAreParsedIfNoEscapeCharacterIsFoundAtTheCurrentPosition()
    {
        var parser = new EscapeCharacterParser();
        var source = "hello";
        var position = 0;

        var result = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(0);
        result.Should().BeNull();
    }

    [Test]
    public void TheCharacterFollowingTheEscapeCharacterIsReturnedRegardlessOfAnySpecialMeaningOfTheCharacter()
    {
        var parser = new EscapeCharacterParser();
        var source = @"\\";
        var position = 0;

        var result = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(2);
        result.Should().Be(@"\");
    }

    [Test]
    public void IfTheEscapeCharacterIsNotFollowedByAnotherCharacterAErrorIsThrown()
    {
        var parser = new EscapeCharacterParser();
        var source = @"\";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after escape-character.");
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA8BitEscapeTheFollowing2CharactersAreInterpretedAsA8BitHexedecimalCodepoint()
    {
        var parser = new EscapeCharacterParser();
        var source = @"\x4C";
        var position = 0;

        var result = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(4);
        result.Should().Be("L");
    }

    [TestCase(@"\x")]
    [TestCase(@"\x1")]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA8BitEscapeAndThereIsLessThan2FollowingCharactersAnErrorIsThrown(string source)
    {
        var parser = new EscapeCharacterParser();
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after 8bit escape-character.");
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA8BitEscapeAndTheFollowing2CharactersAreNotValidHexadecimalDigitsAnErrorIsThrown()
    {
        var parser = new EscapeCharacterParser();
        var source = @"\xhi";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Invalid 8bit escape-character.");
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA16BitEscapeTheFollowing4CharactersAreInterpretedAsA8BitHexedecimalCodepoint()
    {
        var parser = new EscapeCharacterParser();
        var source = @"\u013f";
        var position = 0;

        var result = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(6);
        result.Should().Be("Ŀ");
    }

    [TestCase(@"\u")]
    [TestCase(@"\u1")]
    [TestCase(@"\u12")]
    [TestCase(@"\u123")]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA16BitEscapeAndThereIsLessThan4FollowingCharactersAnErrorIsThrown(string source)
    {
        var parser = new EscapeCharacterParser();
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after 16bit escape-character.");
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA16BitEscapeAndTheFollowing4CharactersAreNotValidHexadecimalDigitsAnErrorIsThrown()
    {
        var parser = new EscapeCharacterParser();
        var source = @"\uhelo";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Invalid 16bit escape-character.");
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA24BitEscapeTheFollowing6CharactersAreInterpretedAsA8BitHexedecimalCodepoint()
    {
        var parser = new EscapeCharacterParser();
        var source = @"\p01d473";
        var position = 0;

        var result = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(8);
        result.Should().Be("𝑳");
    }

    [TestCase(@"\p")]
    [TestCase(@"\p1")]
    [TestCase(@"\p12")]
    [TestCase(@"\p123")]
    [TestCase(@"\p1234")]
    [TestCase(@"\p12345")]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA24BitEscapeAndThereIsLessThan6FollowingCharactersAnErrorIsThrown(string source)
    {
        var parser = new EscapeCharacterParser();
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after 24bit escape-character.");
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA24BitEscapeAndTheFollowing6CharactersAreNotValidHexadecimalDigitsAnErrorIsThrown()
    {
        var parser = new EscapeCharacterParser();
        var source = @"\phello!";
        var position = 0;

        parser.Invoking(p => p.Parse(source, ref position, TestParserConfig.Shared)).Should()
            .Throw<ParseError>().WithMessage("Invalid 24bit escape-character.");
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsASubstitutableCharacterTheSubstitutionIsReturned()
    {
        var parser = new EscapeCharacterParser();
        var source = @"\n";
        var position = 0;

        var result = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(2);
        result.Should().Be("\n");
    }
}