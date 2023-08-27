namespace Jelly.Shell.Extensions;

[TestFixture]
public class StringExtensions
{
    [TestCase("")]
    [TestCase("jello, world")]
    [TestCase("one two three")]
    public void WhenTheStringIsEmptyOrContainsNoLineEndingsAnSingleLineIsReturnedSpanningTheEntireString(string str)
    {
        var lines = str.SplitLines().ToList();

        lines.Single().Should().Be(new Substring(str, 0, str.Length));
        lines.Single().ToString().Should().Be(str);
    }

    [TestCase("jello\nworld", 6)]
    [TestCase("jello\rworld", 6)]
    [TestCase("jello\r\nworld", 7)]
    public void AllCommonLineEndingCanBeDetected(string str, int secondWordPosition)
    {
        var lines = str.SplitLines().ToList();

        lines[0].Should().Be(new Substring(str, 0, 5));
        lines[0].ToString().Should().Be("jello");
        lines[1].Should().Be(new Substring(str, secondWordPosition, str.Length));
        lines[1].ToString().Should().Be("world");
    }

    [Test]
    public void MultipleLinesCanBeSplit()
    {
        var str = "a\nb\nc\nd";

        var lines = str.SplitLines().ToList();

        lines.Count.Should().Be(4);
        lines[0].Should().Be(new Substring(str, 0, 1));
        lines[1].Should().Be(new Substring(str, 2, 3));
        lines[2].Should().Be(new Substring(str, 4, 5));
        lines[3].Should().Be(new Substring(str, 6, 7));
    }

    [Test]
    public void AStringEndingInALineEndingReturnsTwoLines()
    {
        var str = "\n";

        var lines = str.SplitLines().ToList();

        lines.Count.Should().Be(2);
        lines[0].Should().Be(new Substring(str, 0, 0));
        lines[1].Should().Be(new Substring(str, 1, 1));
    }

    [Test]
    public void AnEntireStringCanBeUnderlined()
    {
        var str = "jello, world";

        var underlines = str.Underline(0, str.Length);

        underlines.Single().Should().Be(new UnderlinedText("jello, world", "^^^^^^^^^^^^"));
    }

    [Test]
    public void ASectionOfAStringCanBeUnderlined()
    {
        var str = "jello, world";

        var underlines = str.Underline(5, 7);

        underlines.Single().Should().Be(new UnderlinedText("jello, world", "     ^^"));
    }

    [Test]
    public void ASectionOfAStringCanBeUnderlinedEvenWhenTheLengthOfTheSelectionIsZero()
    {
        var str = "jello, world";

        var underlines = str.Underline(5, 5);

        underlines.Single().Should().Be(new UnderlinedText("jello, world", "     ^"));
    }

    [Test]
    public void TheSectionOfAStringCanBeSetToTheEndOfAString()
    {
        var str = "jello, world";

        var underlines = str.Underline(12, 12);

        underlines.Single().Should().Be(new UnderlinedText("jello, world", "            ^"));
    }

    [Test]
    public void TheHighlightingStartAndEndPositionsAreClampedToBeWithinTheString()
    {
        var str = "jello, world";

        var underlines = str.Underline(-1, str.Length + 1);

        underlines.Single().Should().Be(new UnderlinedText("jello, world", "^^^^^^^^^^^^"));
    }

    [Test]
    public void UnderliningCanSpanMultipleLines()
    {
        var str = "one\ntwo\r\nthree";

        var underlines = str.Underline(2, 13).ToList();

        underlines.Count.Should().Be(3);
        underlines[0].Should().Be(new UnderlinedText("one", "  ^"));
        underlines[1].Should().Be(new UnderlinedText("two", "^^^"));
        underlines[2].Should().Be(new UnderlinedText("three", "^^^^"));
    }

    [Test]
    public void LinesCanBeSkippedAtTheBeginningAndEndOfAStringWhenUnderlining()
    {
        var str = "\n\none\ntwo\r\nthree\rfour\nfive\n\n\n";

        var underlines = str.Underline(6, 16).ToList();

        underlines.Count.Should().Be(2);
        underlines[0].Should().Be(new UnderlinedText("two", "^^^"));
        underlines[1].Should().Be(new UnderlinedText("three", "^^^^^"));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void ConvertingAIndexToALineAndColumnNumberInASingleLineStringReturnsLineOneAndColumnEqualsIndexPlusOne(int index)
    {
        var str = "this is a test";

        var (ln, col) = str.IndexToLineAndColumn(index);

        ln.Should().Be(1);
        col.Should().Be(index + 1);
    }

    [TestCase(-1)]
    [TestCase(-2)]
    public void ConvertingAIndexToALineAndColumnNumberWhenTheIndexIsLessThanZeroReturnsLineOneColumnOne(int index)
    {
        var str = "this is a test";

        var (ln, col) = str.IndexToLineAndColumn(index);

        ln.Should().Be(1);
        col.Should().Be(1);
    }

    [TestCase(0, 1, 1)]
    [TestCase(1, 1, 2)]
    [TestCase(2, 1, 3)]
    [TestCase(3, 1, 4)]
    [TestCase(4, 2, 1)]
    [TestCase(5, 2, 2)]
    [TestCase(6, 2, 3)]
    [TestCase(8, 3, 1)]
    [TestCase(13, 3, 6)]
    public void ConvertingAIndexToALineAndColumnNumberInOnAMultiLineStringReturnsLineIndexesThatCountFromOneAndColumnIndexesThatCountFromTheBeginningOfEachLine(
        int index, int expectedLine, int expectedColumn)
    {
        var str = "one\ntwo\nthree";

        var (ln, col) = str.IndexToLineAndColumn(index);

        ln.Should().Be(expectedLine);
        col.Should().Be(expectedColumn);
    }

    [TestCase(14, 3, 6)]
    [TestCase(15, 3, 6)]
    public void ConvertingAIndexToALineAndColumnNumberInOnAMultiLineStringWhenTheIndexIsGreaterThanTheLengthOfTheStringReturnsTheLastLineAndColumnNumber(
        int index, int expectedLine, int expectedColumn)
    {
        var str = "one\ntwo\nthree";

        var (ln, col) = str.IndexToLineAndColumn(index);

        ln.Should().Be(expectedLine);
        col.Should().Be(expectedColumn);
    }
}