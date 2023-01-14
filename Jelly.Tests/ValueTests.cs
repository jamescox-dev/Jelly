namespace Jelly.Tests;

using Jelly;

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
        new(new Node("node"), "node"),
    };
}