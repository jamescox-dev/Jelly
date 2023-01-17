namespace Jelly.Values.Tests;

using Jelly.Values;

[TestFixture]
public class ListValueTests
{
    [Test]
    public void WhenAListValueHasNoItemsItsStringRepresentationIsAnEmptyString()
    {
        var list = new ListValue();

        var str = list.ToString();

        str.Should().Be("");
    }

    [Test]
    public void AListValuesStringRepresentationContainsTheStringRepresentationOfEachOfItemsInOrderSeparatedBySpaces()
    {
        var list = new ListValue(new Value[]
        {
            "Stan".ToValue(),
            "Ollie".ToValue(),
        });

        var str = list.ToString();

        str.Should().Be("Stan Ollie");
    }
}