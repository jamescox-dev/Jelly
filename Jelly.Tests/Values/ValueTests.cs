namespace Jelly.Values.Tests;

using Jelly.Values;

public class ValueTests
{
    [TestCaseSource(nameof(AllValuesAreStringsTestCaseData))]
    public void AllValuesAreStrings(Value value, string expectedString)
    {
        var str = value.ToString();

        str.Should().Be(expectedString);
    }

    static readonly IReadOnlyList<TestCaseData> AllValuesAreStringsTestCaseData = new List<TestCaseData>
    {
        new(new StringValue("hello, world"), "hello, world"),
    };

    [TestCase("a", "A", -1)]
    [TestCase("Hello", "Hi", -1)]
    [TestCase("same", "same", 0)]
    [TestCase("Hi", "bye", 1)]
    public void ValuesAreComparedAsStringsWithInvariantCulture(string str1, string str2, int expected)
    {
        var value1 = str1.ToValue();
        var value2 = str2.ToValue();
        
        var result = value1.CompareTo(value2);

        result.Should().Be(expected);
    }

    [TestCase("one", "two")]
    [TestCase("three", "four")]
    [TestCase("five", "five")]
    public void ValuesAreEqualIfTheirComparisonToAnotherValueIsZero(string str1, string str2)
    {
        var value1 = str1.ToValue();
        var value2 = str2.ToValue();
        var comparison = value1.CompareTo(value2);

        var equals = value1.Equals(value2);

        equals.Should().Be(comparison == 0);
    }

    [Test]
    public void ValuesComparedForEqualityAgainstNonValueTypesAreNotEqual()
    {
        var value = "0".ToValue();

        value.Equals(0).Should().BeFalse();
        value.Equals("0").Should().BeFalse();
        value.Equals(null).Should().BeFalse();
    }

    [Test]
    public void WhenValuesComparedForEqualityAreBothValueTypesTheValueSpecificEqualsMethodIsUsed()
    {
        var value1 = "test".ToValue();
        object value2 = "test".ToValue();

        var equal = value1.Equals(value2);

        equal.Should().BeTrue();
    }

    [Test]
    public void WhenAValueIsComparedToANullValueTheNullValueIsTreatedAsAnEmptyString()
    {
        var empty = Value.Empty;

        var comparison = empty.CompareTo(null);

        comparison.Should().Be(0);
    }

    [TestCase("Homer")]
    [TestCase("Marge")]
    [TestCase("Bart")]
    [TestCase("Lisa")]
    [TestCase("Maggie")]
    public void ValuesHashCodesAreCalculatedWithTheInvariantCulture(string str)
    {
        var value = str.ToValue();

        var hash = value.GetHashCode();

        hash.Should().Be(StringComparer.InvariantCulture.GetHashCode(str));
    }

    [Test]
    public void ValueProvidesAConstantEmptyValue()
    {
        Value.Empty.ToString().Should().Be(string.Empty);
    }
}