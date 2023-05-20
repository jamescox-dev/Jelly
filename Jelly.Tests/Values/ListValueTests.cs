namespace Jelly.Values.Tests;

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

    [TestCase("0")]
    [TestCase("false")]
    [TestCase("0.0")]
    public void SingleItemListWithFalseyValuesConvertToBoolFalse(string item)
    {
        var list = new ListValue(item.ToValue());

        var b = list.ToBool();

        b.Should().BeFalse();
    }

    [TestCase("")]
    [TestCase("false 0.0")]
    [TestCase("true")]
    public void EmptyListsOrListsWithMoreThanOneValueConvertToBoolTrue(string listString)
    {
        var list = new ListParser().Parse(new Scanner(listString));

        var b = list.ToBool();

        b.Should().BeTrue();
    }

    [TestCase("0", 0.0)]
    [TestCase("false", 0.0)]
    [TestCase("true", 1.0)]
    [TestCase("42", 42.0)]
    [TestCase("", double.NaN)]
    public void SingleItemListWithConvertToTheDoubleValueOfTheFirstItem(string item, double expectedValue)
    {
        var list = new ListValue(item.ToValue());

        var d = list.ToDouble();

        d.Should().Be(expectedValue);
    }

    [TestCase("")]
    [TestCase("false 0.0")]
    [TestCase("1 2 3")]
    public void EmptyListsOrListsWithMoreThanOneValueDoNotConvertToNumbers(string listString)
    {
        var list = new ListParser().Parse(new Scanner(listString));

        var d = list.ToDouble();

        double.IsNaN(d).Should().BeTrue();
    }
}