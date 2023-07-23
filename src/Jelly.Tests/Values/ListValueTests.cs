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

        var item = list[0];

        item.Should().Be("a".ToValue());
    }

    [Test]
    public void IfAnIndexIsBelowZeroAnIndexErrorIsRaised()
    {
        var list = new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue());
        var action = () => list[-1];

        action.Should().Throw<IndexError>().WithMessage("index out of bounds.");
    }

    [Test]
    public void IfAnIndexIsGreaterOrEqualToTheLengthOfTheList()
    {
        var list = new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue());
        var equal = () => list[3];
        var greater = () => list[4];

        equal.Should().Throw<IndexError>().WithMessage("index out of bounds.");
        greater.Should().Throw<IndexError>().WithMessage("index out of bounds.");
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

        ((Value)dict).Should().Be(new DictValue("a".ToValue(), "b".ToValue(), "c".ToValue(), "d".ToValue()));
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

    [Test]
    public void AnItemCanBeAddedToAList()
    {
        var list = new ListValue("a".ToValue());

        var newList = list.Add("z".ToValue());

        ((Value)newList).Should().Be(new ListValue("a".ToValue(), "z".ToValue()));
    }

    [Test]
    public void AnItemInTheListCanBeChanged()
    {
        var list = new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue());

        var newList = list.SetItem(1, "x".ToValue());

        ((Value)newList).Should().Be(new ListValue("a".ToValue(), "x".ToValue(), "c".ToValue()));
    }

    [Test]
    public void AnListCanBeAddedToAList()
    {
        var list = new ListValue("a".ToValue());

        var newList = list.AddRange(new ListValue("b".ToValue(), "c".ToValue()));

        ((Value)newList).Should().Be(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue()));
    }

    [Test]
    public void IfAnIndexIsBelowZeroWhenSettingAnItemAnIndexErrorIsRaised()
    {
        var list = new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue());
        var action = () => list.SetItem(-1, "x".ToValue());

        action.Should().Throw<IndexError>().WithMessage("index out of bounds.");
    }

    [Test]
    public void IfAnIndexIsGreaterOrEqualToTheLengthOfTheListWhenSettingAnItem()
    {
        var list = new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue());
        var equal = () => list.SetItem(3, "x".ToValue());
        var greater = () => list.SetItem(4, "x".ToValue());

        equal.Should().Throw<IndexError>().WithMessage("index out of bounds.");
        greater.Should().Throw<IndexError>().WithMessage("index out of bounds.");
    }
}