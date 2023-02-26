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
    public void AListValuesStringRepresentationContainsTheStringRepresentationOfEachOfItemsInOrderSeparatedByNewLines()
    {
        var list = new ListValue(new Value[]
        {
            "Stan".ToValue(),
            "Ollie".ToValue(),
        });

        var str = list.ToString();

        str.Should().Be("Stan\nOllie");
    }

    [Test]
    public void EachItemInTheListValuesStringRepresentationIsEscaped()
    {
        var list = new ListValue(new Value[]
        {
            "jello, world".ToValue(),
        });

        var str = list.ToString();

        str.Should().Be("'jello, world'");
    }

    [Test]
    public void AListValueReturnsItselfWhenAskedToConvertToAList()
    {
        Value value = new ListValue();

        var list = value.ToListValue();

        ((IComparable<Value>)list).Should().BeSameAs(value);
    }

    [Test]
    public void AValueCanBeRetrievedFromAListValueViaItsIndex()
    {
        var list = new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue());

        var item = list[1];

        item.Should().Be("b".ToValue());
    }

    [Test]
    public void TheNumberOfItemsInAListCanBeRetrieved()
    {
        var list = new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue());

        var count = list.Count;

        count.Should().Be(3);
    }

    [Test]
    public void ListValuesCanBeConvertedToDictionaryValues()
    {
        var value = new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue(), "d".ToValue());

        var dict = value.ToDictionaryValue();

        ((Value)dict).Should().Be(new DictionaryValue("a".ToValue(), "b".ToValue(), "c".ToValue(), "d".ToValue()));
    }
}