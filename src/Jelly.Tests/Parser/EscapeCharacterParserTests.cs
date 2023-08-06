


namespace Jelly.Parser.Tests;

[TestFixture]
public class EscapeCharacterParserTests
{
    [Test]
    public void NoCharactersAreParsedIfNoEscapeCharacterIsFoundAtTheCurrentPosition()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner("hello");

        var result = parser.Parse(scanner);

        scanner.Position.Should().Be(0);
        result.Should().BeNull();
    }

    [Test]
    public void TheCharacterFollowingTheEscapeCharacterIsReturnedRegardlessOfAnySpecialMeaningOfTheCharacter()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(@"\\");

        var result = parser.Parse(scanner);

        scanner.Position.Should().Be(2);
        result.Should().Be(@"\");
    }

    [Test]
    public void IfTheEscapeCharacterIsNotFollowedByAnotherCharacterAErrorIsThrown()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(@"\");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after escape-character.")
            .Where(e => e.StartPosition == 0 && e.EndPosition == 1);
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA8BitEscapeTheFollowing2CharactersAreInterpretedAsA8BitHexedecimalCodepoint()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(@"\x4C");

        var result = parser.Parse(scanner);

        scanner.Position.Should().Be(4);
        result.Should().Be("L");
    }

    [TestCase(@"\x")]
    [TestCase(@"\x1")]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA8BitEscapeAndThereIsLessThan2FollowingCharactersAnErrorIsThrown(string source)
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(source);

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after 8bit escape-character.")
            .Where(e => e.StartPosition == 0 && e.EndPosition == source.Length);
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA8BitEscapeAndTheFollowing2CharactersAreNotValidHexadecimalDigitsAnErrorIsThrown()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(@"\xhi");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Invalid 8bit escape-character.")
            .Where(e => e.StartPosition == 0 && e.EndPosition == 4);
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA16BitEscapeTheFollowing4CharactersAreInterpretedAsA8BitHexedecimalCodepoint()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(@"\u013f");

        var result = parser.Parse(scanner);

        scanner.Position.Should().Be(6);
        result.Should().Be("Ŀ");
    }

    [TestCase(@"\u")]
    [TestCase(@"\u1")]
    [TestCase(@"\u12")]
    [TestCase(@"\u123")]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA16BitEscapeAndThereIsLessThan4FollowingCharactersAnErrorIsThrown(string source)
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(source);

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after 16bit escape-character.")
            .Where(e => e.StartPosition == 0 && e.EndPosition == source.Length);
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA16BitEscapeAndTheFollowing4CharactersAreNotValidHexadecimalDigitsAnErrorIsThrown()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(@"\uhelo");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Invalid 16bit escape-character.")
            .Where(e => e.StartPosition == 0 && e.EndPosition == 6);
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA24BitEscapeTheFollowing6CharactersAreInterpretedAsA8BitHexedecimalCodepoint()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(@"\p01d473");

        var result = parser.Parse(scanner);

        scanner.Position.Should().Be(8);
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
        var scanner = new Scanner(source);

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-input after 24bit escape-character.")
            .Where(e => e.StartPosition == 0 && e.EndPosition == source.Length);
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsA24BitEscapeAndTheFollowing6CharactersAreNotValidHexadecimalDigitsAnErrorIsThrown()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(@"\phello!");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Invalid 24bit escape-character.")
            .Where(e => e.StartPosition == 0 && e.EndPosition == 8);
    }

    [Test]
    public void IfTheCharacterFollowingTheEscapeCharacterIsASubstitutableCharacterTheSubstitutionIsReturned()
    {
        var parser = new EscapeCharacterParser();
        var scanner = new Scanner(@"\n");

        var result = parser.Parse(scanner);

        scanner.Position.Should().Be(2);
        result.Should().Be("\n");
    }
}