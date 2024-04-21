namespace Jelly.Tests.Values;

[TestFixture]
public class DictValueTests
{
    [Test]
    public void WhenADictionaryValueHasNoItemsItsStringRepresentationIsAnEmptyString()
    {
        var dict = new DictValue();

        var str = dict.ToString();

        str.Should().Be("");
    }

    [Test]
    public void ADictionaryValuesStringRepresentationContainsTheStringRepresentationOfEachOfItsKeysAndAssociatedValueSeparatedBySpacesInAlphabeticalOrderByWithEachPairSeparatedByNewlines()
    {
        var dict = new DictValue(
            "zProp".ToValue(), "26".ToValue(),
            "aProp".ToValue(), "1".ToValue());

        var str = dict.ToString();

        str.Should().Be("aProp 1\nzProp 26");
    }

    [Test]
    public void EachKeyAndValueIsEscapedInTheDictionariesStringRepresentation()
    {
        var dict = new DictValue(
            "friendly greeting".ToValue(), "jello, world".ToValue());

        var str = dict.ToString();

        str.Should().Be("'friendly greeting' 'jello, world'");
    }

    [Test]
    public void AValueFromTheDictionaryCanBeRetrievedWithAKey()
    {
        var dict = new DictValue(
            "name".ToValue(), "James".ToValue(),
            "age".ToValue(), "38".ToValue());

        var name = dict["name".ToValue()];
        var age = dict["age".ToValue()];

        name.Should().Be("James".ToValue());
        age.Should().Be("38".ToValue());
    }

    [Test]
    public void WhenTryingToGetAValueWithAKeyThatExistsTrueIsReturnedAndTheValueIsSet()
    {
        var dict = new DictValue(
            "name".ToValue(), "James".ToValue(),
            "age".ToValue(), "38".ToValue());

        var success = dict.TryGetValue("name".ToValue(), out var name);

        success.Should().BeTrue();
        name.Should().Be("James".ToValue());
    }

    [Test]
    public void WhenTryingToGetAValueWithAKeyThatDoesNotExistsFalseIsReturnedAndTheValueIsSetToEmpty()
    {
        var dict = new DictValue(
            "name".ToValue(), "James".ToValue(),
            "age".ToValue(), "38".ToValue());

        var success = dict.TryGetValue("unknown".ToValue(), out var name);

        success.Should().BeFalse();
        name.Should().Be(Value.Empty);
    }

    [Test]
    public void ADictionaryValueReturnsItselfWhenAskedToConvertToADictionary()
    {
        Value value = new DictValue();

        var dict = value.ToDictValue();

        dict.Should().BeSameAs(value);
    }

    [TestCase("name", true)]
    [TestCase("height", false)]
    public void TheDictionaryValueCanBeQueriedForAKeysExistence(string key, bool expected)
    {
        var dict = new DictValue(
            "name".ToValue(), "James".ToValue(),
            "age".ToValue(), "38".ToValue());

        var exists = dict.ContainsKey(key.ToValue());

        exists.Should().Be(expected);
    }

    [Test]
    public void DictionaryValuesCanBeConvertedToListValues()
    {
        var value = new DictValue("a".ToValue(), "b".ToValue(), "c".ToValue());

        var list = value.ToListValue();

        ((Value)list).Should().Be(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue(), Value.Empty));
    }

    [Test]
    public void ADictionaryDoesNotConvertToANumber()
    {
        var dict1 = new DictValue();
        var dict2 = new DictValue(Value.Empty);
        var dict3 = new DictValue(new NumValue(1.0), " ".ToValue());

        double.IsNaN(dict1.ToDouble()).Should().BeTrue();
        double.IsNaN(dict2.ToDouble()).Should().BeTrue();
        double.IsNaN(dict3.ToDouble()).Should().BeTrue();
    }

    [Test]
    public void ADictionaryAlwaysConvertsToATrueBool()
    {
        var dict1 = new DictValue();
        var dict2 = new DictValue(Value.Empty);
        var dict3 = new DictValue(new NumValue(1.0), " ".ToValue());

        dict1.ToBool().Should().BeTrue();
        dict2.ToBool().Should().BeTrue();
        dict3.ToBool().Should().BeTrue();
    }

    [Test]
    public void AValueCanBeRetrievedFromADictionaryByKey()
    {
        var dict = new DictValue("a".ToValue(), 1.ToValue());

        var value = dict["a".ToValue()];

        value.Should().Be(1.ToValue());
    }

    [Test]
    public void AItemCanBeRemovedFromADictionaryByItsKey()
    {
        var dict1 = new DictValue("a".ToValue(), 1.ToValue());

        var dict2 = dict1.RemoveRange("a".ToValue());

        dict2.Should().Be(DictValue.Empty);
    }

    [Test]
    public void MultipleItemsCanBeRemovedFromADictionaryByItsKey()
    {
        var dict1 = new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 1.ToValue());

        var dict2 = dict1.RemoveRange("a".ToValue(), "b".ToValue());

        dict2.Should().Be(DictValue.Empty);
    }

    [Test]
    public void WhenRemovingAnItemByItsKeyIfTheKeyDoesNotExistNoItemIsRemoved()
    {
        var dict1 = new DictValue("a".ToValue(), 1.ToValue());

        var dict2 = dict1.RemoveRange("b".ToValue());

        dict1.Should().Be(dict1);
    }

    [Test]
    public void WhenRetrievingAValueFromADictionaryByAKeyTheDoesNotExistAnErrorIsThrown()
    {
        var dict = new DictValue("a".ToValue(), 1.ToValue());

        dict.Invoking(d => d["b".ToValue()]).Should()
            .Throw<KeyError>().WithMessage("key does not exist in dictionary.");
    }

    [Test]
    public void AValueCanBeSetByKeyInADictionary()
    {
        var dict1 = new DictValue("a".ToValue(), 1.ToValue());

        var dict2 = dict1.SetItem("b".ToValue(), 2.ToValue());

        dict2.Should().Be(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue()));
    }

    [Test]
    public void MultipleValuesCanBeSetByKeyInADictionary()
    {
        var dict1 = new DictValue("a".ToValue(), 1.ToValue());

        var dict2 = dict1.SetItems(new DictValue("a".ToValue(), 2.ToValue(), "b".ToValue(), 1.ToValue()).ToEnumerable());

        dict2.Should().Be(new DictValue("a".ToValue(), 2.ToValue(), "b".ToValue(), 1.ToValue()));
    }

    [Test]
    public void TheNumberOfItemsInADictionaryCanBeRetrieved()
    {
        var dict0 = new DictValue();
        var dict1 = new DictValue("a".ToValue(), 1.ToValue());
        var dict2 = new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 1.ToValue());

        var count0 = dict0.Count;
        var count1 = dict1.Count;
        var count2 = dict2.Count;

        count0.Should().Be(0);
        count1.Should().Be(1);
        count2.Should().Be(2);
    }
}