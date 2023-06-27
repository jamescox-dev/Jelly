namespace Jelly.Commands.ArgParsers.Tests;

[TestFixture]
public class KeywordArgPatternTests
{
    [Test]
    public void IfTheArgListContainsOnlyOneMatchingLiteralNodeAndItIsParsedFromTheBeginningSuccessIsReturnedWithCorrectPosition()
    {
        var args = new ListValue(Node.Literal("key_test"));
        var pattern = new KeywordArgPattern("key_test");

        var result = pattern.Parse(0, args);

        result.Should().BeOfType<ArgPatternSuccess>().Which.Position.Should().Be(1);
        result.Should().BeOfType<ArgPatternSuccess>()
            .Which.ArgValues.Should().BeEquivalentTo(new Dictionary<string, Value>());
    }

    [Test]
    public void IfTheArgListContainsOnlyItemAndItIsNotAMatchingLiteralNodeAndItIsParsedFromTheBeginningMissingIsReturnedWithCorrectPosition()
    {
        var args = new ListValue(Node.Literal("not_match"));
        var pattern = new KeywordArgPattern("another_key");

        var result = pattern.Parse(0, args);

        result.Should().BeOfType<ArgPatternResultMissing>().Which.Position.Should().Be(0);
        result.Should().BeOfType<ArgPatternResultMissing>()
            .Which.MissingArgs.Should().BeEquivalentTo(new HashSet<Arg>{ new KwArg("another_key") });
    }

    [Test]
    public void KeywordsAreMatchedCaseInsensitively()
    {
        var args = new ListValue(Node.Literal("Case_Insensitive"));
        var pattern = new KeywordArgPattern("case_insensitive");

        var result = pattern.Parse(0, args);

        result.Should().BeOfType<ArgPatternSuccess>().Which.Position.Should().Be(1);
        result.Should().BeOfType<ArgPatternSuccess>()
            .Which.ArgValues.Should().BeEquivalentTo(new Dictionary<string, Value>());
    }

    [Test]
    public void IfTheArgListIsEmptyAndParsedFromTheBeginningMissingIsReturnedWithTheCorrectArgAndPosition()
    {
        var args = ListValue.EmptyList;
        var pattern = new KeywordArgPattern("keyword");

        var result = pattern.Parse(0, args);

        result.Should().BeOfType<ArgPatternResultMissing>().Which.Position.Should().Be(0);
        result.Should().BeOfType<ArgPatternResultMissing>()
            .Which.MissingArgs.Should().BeEquivalentTo(new HashSet<Arg>{ new KwArg("keyword") });
    }

    [Test]
    public void IfTheArgListIsParsedAtTheEndMissingIsReturnedWithTheCorrectArgAndPosition()
    {
        var args = new ListValue("test_value".ToValue());
        var pattern = new KeywordArgPattern("more_words");

        var result = pattern.Parse(1, args);

        result.Should().BeOfType<ArgPatternResultMissing>().Which.Position.Should().Be(1);
        result.Should().BeOfType<ArgPatternResultMissing>()
            .Which.MissingArgs.Should().BeEquivalentTo(new HashSet<Arg>{ new Arg("more_words") });
    }

    [TestCase(0, "one")]
    [TestCase(1, "two")]
    [TestCase(2, "three")]
    public void TheKeywordListCanBeParsedFromAnyPosition(int position, string keyword)
    {
        var args = new ListValue(Node.Literal("one"), Node.Literal("two"), Node.Literal("three"));
        var pattern = new KeywordArgPattern(keyword);

        var result = pattern.Parse(position, args);

        result.Should().BeOfType<ArgPatternSuccess>().Which.Position.Should().Be(position + 1);
        result.Should().BeOfType<ArgPatternSuccess>()
            .Which.ArgValues.Should().BeEquivalentTo(new Dictionary<string, Value>());
    }
}