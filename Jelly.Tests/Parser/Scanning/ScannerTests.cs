namespace Jelly.Parser.Scanning.Tests;

[TestFixture]
public class ScannerTests
{
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(4)]
    public void TheScannerCanBeAdvancedByTheGivenNumberOfCharacters(int ammount)
    {
        var scanner = new Scanner("scan me!", 2);

        scanner.Advance(ammount);

        scanner.Position.Should().Be(2 + ammount);
    }

    [Test]
    public void WhenTheScannerIsAdvancedAnNoAmountOfCharactersIsSpecifiedThePositionIsIncrementedByOne()
    {
        var scanner = new Scanner("scan me!", 5);

        scanner.Advance();

        scanner.Position.Should().Be(6);
    }

    [TestCase(1, 1, 2)]
    [TestCase(1, 7, 0)]
    [TestCase(1, 1, 4)]
    [TestCase(1, 0, 0)]
    public void TheScannerCanBeConditionallyAdvancedWithAPredicateAndReturnsWeatherThePredicateMatches(int a, int b, int amount)
    {
        var scanner = new Scanner("scan me!");

        var result = scanner.AdvanceIf(s => a == b, amount);

        scanner.Position.Should().Be(amount);
        result.Should().Be(scanner.Position > 0);
    }

    [TestCase(" boo!", 0)]
    [TestCase(" ah!", 1)]
    [TestCase(" aaah!", 3)]
    [TestCase(" aaaaaah!", 6)]
    public void TheScannerCanConditionallyAdvancesWhileAPredicateIsTrueAndReturnsHowMuchTheScannerHasBeenAdvancedBy(string source, int expectedAdvancement)
    {
        var scanner = new Scanner(source, 1);

        var advacnedBy = scanner.AdvanceWhile(s => s.CurrentCharacter == 'a');

        advacnedBy.Should().Be(expectedAdvancement);
        scanner.Position.Should().Be(expectedAdvancement + 1);
    }

    [Test]
    public void IfTheScannerStopsAdvanceingOnceTheEndOfTheInputIsReachedDispiteWeatherThePredicateRemainsTrue()
    {
        var i = 0;
        var scanner = new Scanner("this is all good!", 4);

        var advacnedBy = scanner.AdvanceWhile(s => ++i < 100);

        advacnedBy.Should().Be(13);
        scanner.Position.Should().Be(17);
    }

    [TestCase("Test", 4)]
    [TestCase("Test", 5)]
    public void WhenThePositionAtOrBeyondTheEndOfTheSourceAEofIsReported(string source, int position)
    {
        var scanner = new Scanner(source, position);

        var eof = scanner.IsEof;

        eof.Should().BeTrue();
    }

    [TestCase("Test", 0)]
    [TestCase("Test", 2)]
    public void WhenThePositionIsBeforeTheEndOfTheSourceAEofIsNotReported(string source, int position)
    {
        var scanner = new Scanner(source, position);

        var eof = scanner.IsEof;

        eof.Should().BeFalse();
    }

    [TestCase(-1, null)]
    [TestCase(0, 'h')]
    [TestCase(5, ',')]
    [TestCase(7, 'w')]
    [TestCase(11, 'd')]
    [TestCase(12, null)]
    public void TheCurrentCharacterAtCurrentPositionCanBeRetrivedOrNullIfOutOfBounds(int position, char? expected)
    {
        var scanner = new Scanner("hello, world", position);

        var ch = scanner.CurrentCharacter;

        ch.Should().Be(expected);
    }

    [TestCase("hello, world", 7, 5, "world")]
    [TestCase("substring", 3, 6, "string")]
    [TestCase("choo-choo", 6, 10, "hoo")]
    [TestCase("test", 4, 1, "")]
    [TestCase("egg and chips", 0, 13, "egg and chips")]
    public void ASubstringOfAGivenLengthCanBeExtractedFromTheCurrentPositionAndIsTrucatedIfLongerThanTheSource(string source, int position, int length, string expected)
    {
        var scanner = new Scanner(source, position);

        var substring = scanner.Substring(length);

        substring.Should().Be(expected);
    }

    [TestCase(@"\escape", true)]
    [TestCase("noescape", false)]
    public void AnEscapeCharacterIsReportedWhenTheCurrentCharacterMatchesTheConfiguredEscapeCharacter(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsEscapeCharacter.Should().Be(expected);
    }

    [TestCase(@"\x", true)]
    [TestCase(@"\X", true)]
    [TestCase(@"\\", false)]
    public void AnEscapeCharacter8bitIsReportedWhenTheCurrentCharacterMatchesOneOfTheConfiguredEscapeCharacters8bit(string source, bool expected)
    {
        var scanner = new Scanner(source, 1);

        scanner.IsEscapeCharacter8bit.Should().Be(expected);
    }

    [TestCase(@"\u", true)]
    [TestCase(@"\U", true)]
    [TestCase(@"\\", false)]
    public void AnEscapeCharacter16bitIsReportedWhenTheCurrentCharacterMatchesOneOfTheConfiguredEscapeCharacters16bit(string source, bool expected)
    {
        var scanner = new Scanner(source, 1);

        scanner.IsEscapeCharacter16bit.Should().Be(expected);
    }

    [TestCase(@"\p", true)]
    [TestCase(@"\P", true)]
    [TestCase(@"\\", false)]
    public void AnEscapeCharacter24bitIsReportedWhenTheCurrentCharacterMatchesOneOfTheConfiguredEscapeCharacters16bit(string source, bool expected)
    {
        var scanner = new Scanner(source, 1);

        scanner.IsEscapeCharacter24bit.Should().Be(expected);
    }

    [TestCase("n", true)]
    [TestCase("0", true)]
    [TestCase("t", true)]
    [TestCase("-", false)]
    public void AnEscapeSubstitutableCharacterIsReportedWhenTheCurrentCharacterMatchesOneOfTheConfiguredEscapeSubstitutableCharacters(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsEscapeSubstitutableCharacter.Should().Be(expected);
    }

    [TestCase(@"\n", '\n')]
    [TestCase(@"\r", '\r')]
    [TestCase(@"\t", '\t')]
    [TestCase(@"\z", 'z')]
    public void AnEscapeCharactersSubstitutionCanBeRetrievedForTheGivenPositionBasedOnConfigurationIfNoSubstitionExistsTheCurrentCharacterIsReturned(string source, char? expected)
    {
        var scanner = new Scanner(source, 1);

        var ch = scanner.SubstitedEscapeCharacter;

        ch.Should().Be(expected);
    }

    [TestCase(-1)]
    [TestCase(4)]
    public void IfThePositionIsOutOfBoundsWhenGettingAnEscapeCharactersSubstitutionANullIsReturned(int position)
    {
        var scanner = new Scanner("test", position);

        var ch = scanner.SubstitedEscapeCharacter;

        ch.Should().BeNull();
    }

    [TestCase("\n", true)]
    [TestCase(";", true)]
    [TestCase("boo", false)]
    public void ACommandSeparatorIsReportedWhenTheCurrentCharacterMatchesTheConfiguredACommandSeparator(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsCommandSeparator.Should().Be(expected);
    }

    [TestCase(" ", true)]
    [TestCase("\t", true)]
    [TestCase("boo", false)]
    public void AWordSeparatorIsReportedWhenTheCurrentCharacterMatchesTheConfiguredWordSeparator(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsWordSeparator.Should().Be(expected);
    }

    [TestCase(" ", true)]
    [TestCase("\t", true)]
    [TestCase("\n", true)]
    [TestCase("\r", true)]
    [TestCase("boo", false)]
    public void AListItemSeparatorIsReportedWhenTheCurrentCharacterMatchesTheConfiguredListItemSeparator(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsListItemSeparator.Should().Be(expected);
    }

    [TestCase(" ", true)]
    [TestCase("\t", true)]
    [TestCase("\n", true)]
    [TestCase("\r", true)]
    [TestCase("boo", false)]
    public void AExpressionWordSeparatorIsReportedWhenTheCurrentCharacterMatchesTheConfiguredExpressionWordSeparator(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsExpressionWordSeparator.Should().Be(expected);
    }

    [TestCase("#", true)]
    [TestCase("boo", false)]
    public void ACommentBeginIsReportedWhenTheCurrentCharacterMatchesTheConfiguredCommentBegin(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsCommentBegin.Should().Be(expected);
    }

    [TestCase("\n", true)]
    [TestCase("boo", false)]
    public void ACommentEndIsReportedWhenTheCurrentCharacterMatchesTheConfiguredCommentEnd(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsCommentEnd.Should().Be(expected);
    }

    [TestCase("[", true)]
    [TestCase("boo", false)]
    public void ANestingQuoteBeginIsReportedWhenTheCurrentCharacterMatchesTheConfiguredNestingQuoteBegin(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsNestingQuoteBegin.Should().Be(expected);
    }

    [TestCase("]", true)]
    [TestCase("boo", false)]
    public void ANestingQuoteEndIsReportedWhenTheCurrentCharacterMatchesTheConfiguredNestingQuoteEnd(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsNestingQuoteEnd.Should().Be(expected);
    }

    [TestCase("'", true)]
    [TestCase("\"", true)]
    [TestCase("boo", false)]
    public void AQuoteIsReportedWhenTheCurrentCharacterMatchesAnyOfTheConfiguredQuotes(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsQuote.Should().Be(expected);
    }

    [TestCase("(", true)]
    [TestCase("boo", false)]
    public void AExpressionBeginIsReportedWhenTheCurrentCharacterMatchesTheConfiguredExpresionBegin(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsExpressionBegin.Should().Be(expected);
    }

    [TestCase(")", true)]
    [TestCase("boo", false)]
    public void AExpressionEndIsReportedWhenTheCurrentCharacterMatchesTheConfiguredExpresionEnd(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsExpressionEnd.Should().Be(expected);
    }

    [TestCase("{", true)]
    [TestCase("boo", false)]
    public void AScriptBeginIsReportedWhenTheCurrentCharacterMatchesTheConfiguredScriptBegin(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsScriptBegin.Should().Be(expected);
    }

    [TestCase("}", true)]
    [TestCase("boo", false)]
    public void AScriptEndIsReportedWhenTheCurrentCharacterMatchesTheConfiguredScriptEnd(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsScriptEnd.Should().Be(expected);
    }

    [TestCase("$", true)]
    [TestCase("boo", false)]
    public void AVariableMarkerIsReportedWhenTheCurrentCharacterMatchesTheConfiguredVariableMarker(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsVariableMarker.Should().Be(expected);
    }

    [TestCase("{", true)]
    [TestCase("boo", false)]
    public void AVariableBeginIsReportedWhenTheCurrentCharacterMatchesTheConfiguredVariableBegin(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsVariableBegin.Should().Be(expected);
    }

    [TestCase("}", true)]
    [TestCase("boo", false)]
    public void AVariableEndIsReportedWhenTheCurrentCharacterMatchesTheConfiguredVariableEnd(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsVariableEnd.Should().Be(expected);
    }

    [TestCase("$", true)]
    [TestCase("{", true)]
    //[TestCase("}", true)]
    [TestCase("(", true)]
    //[TestCase(")", true)]
    [TestCase("[", true)]
    //[TestCase("]", true)]
    [TestCase("\\", true)]
    [TestCase(" ", true)]
    [TestCase(";", true)]
    [TestCase("'", true)]
    [TestCase("boo", false)]
    public void AnyCharacterWithSpecialMeaningCanBeReported(string source, bool expected)
    {
        var scanner = new Scanner(source);

        scanner.IsSpecialCharacter.Should().Be(expected);
    }

    [TestCase("A", 1, "A", 1, true)]
    [TestCase("A", 1, "B", 1, false)]
    [TestCase("A", 1, "A", 2, false)]
    [TestCase("A", 1, "B", 2, false)]
    public void ScannersWithTheSameSourceAndPositionAreEqual(string src1, int pos1, string src2, int pos2, bool expectedEqual)
    {
        var scanner1 = new Scanner(src1, pos1);
        var scanner2 = new Scanner(src2, pos2);
        
        var equals = scanner1.Equals(scanner2);

        equals.Should().Be(expectedEqual);
    }

    [TestCase("Source1", 4)]
    [TestCase("Source1", 5)]
    [TestCase("Something", 3)]
    [TestCase("Else", 3)]
    public void AUniqueHashCodeIsGeneratedForEachSourcePostionCombination(string source, int position)
    {
        var scanner1 = new Scanner(source, position);
        var scanner2 = new Scanner("Unique", 1);
    
        var hashCode = scanner1.GetHashCode();

        hashCode.Should().NotBe(scanner2.GetHashCode());
    }
}