namespace Jelly.Commands.ArgParsers.Tests;

[TestFixture]
public class SingleArgPatternTests
{
    [Test]
    public void IfTheArgListContainsOnlyOneItemAndItIsParsedFromTheBeginningSuccessIsReturnedWithTheCorrectArgValuesAndPosition()
    {
        var args = new ListValue("test_value".ToValue());
        var pattern = new SingleArgPattern("test_arg");

        var result = pattern.Parse(0, args);

        result.Should().BeOfType<ArgPatternSuccess>().Which.Position.Should().Be(1);
        result.Should().BeOfType<ArgPatternSuccess>()
            .Which.ArgValues.Should().BeEquivalentTo(new Dictionary<string, Value>
            {
                { "test_arg", "test_value".ToValue() }
            });
    }

    [Test]
    public void IfTheArgListIsEmptyAndParsedFromTheBeginningMissingIsReturnedWithTheCorrectArgAndPosition()
    {
        var args = ListValue.EmptyList;
        var pattern = new SingleArgPattern("another_test");

        var result = pattern.Parse(0, args);

        result.Should().BeOfType<ArgPatternMissing>().Which.Position.Should().Be(0);
        result.Should().BeOfType<ArgPatternMissing>()
            .Which.MissingArgs.Should().BeEquivalentTo(new HashSet<Arg>{ new Arg("another_test") });
    }

    [Test]
    public void IfTheArgListIsParsedAtTheEndMissingIsReturnedWithTheCorrectArgAndPosition()
    {
        var args = new ListValue("test_value".ToValue());
        var pattern = new SingleArgPattern("yet_another");

        var result = pattern.Parse(1, args);

        result.Should().BeOfType<ArgPatternMissing>().Which.Position.Should().Be(1);
        result.Should().BeOfType<ArgPatternMissing>()
            .Which.MissingArgs.Should().BeEquivalentTo(new HashSet<Arg>{ new Arg("yet_another") });
    }

    [TestCase(0, "one")]
    [TestCase(1, "two")]
    [TestCase(2, "three")]
    public void TheArgListCanBeParsedFromAnyPosition(int position, string expectedValue)
    {
        var args = new ListValue("one".ToValue(), "two".ToValue(), "three".ToValue());
        var pattern = new SingleArgPattern("anyway");

        var result = pattern.Parse(position, args);

        result.Should().BeOfType<ArgPatternSuccess>().Which.Position.Should().Be(position + 1);
        result.Should().BeOfType<ArgPatternSuccess>()
            .Which.ArgValues.Should().BeEquivalentTo(new Dictionary<string, Value>
            {
                { "anyway", expectedValue.ToValue() }
            });
    }
}