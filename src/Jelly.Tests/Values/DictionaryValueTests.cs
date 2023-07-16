namespace Jelly.Values.Tests;

[TestFixture]
public class DictionaryValueTests
{
    [Test]
    public void WhenADictionaryValueHasNoItemsItsStringRepresentationIsAnEmptyString()
    {
        var dict = new DictionaryValue();

        var str = dict.ToString();

        str.Should().Be("");
    }

    [Test]
    public void ADictionaryValuesStringRepresentationContainsTheStringRepresentationOfEachOfItsKeysAndAssociatedValueSeparatedBySpacesInAlphabeticalOrderByWithEachPairSeparatedByNewlines()
    {
        var dict = new DictionaryValue(
            "zProp".ToValue(), "26".ToValue(),
            "aProp".ToValue(), "1".ToValue());

        var str = dict.ToString();

        str.Should().Be("aProp 1\nzProp 26");
    }

    [Test]
    public void EachKeyAndValueIsEscapedInTheDictionariesStringRepresentation()
    {
        var dict = new DictionaryValue(
            "friendly greeting".ToValue(), "jello, world".ToValue());

        var str = dict.ToString();

        str.Should().Be("'friendly greeting' 'jello, world'");
    }

    [Test]
    public void AValueFromTheDictionaryCanBeRetrievedWithAKey()
    {
        var dict = new DictionaryValue(
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
        var dict = new DictionaryValue(
            "name".ToValue(), "James".ToValue(),
            "age".ToValue(), "38".ToValue());

        var success = dict.TryGetValue("name".ToValue(), out var name);

        success.Should().BeTrue();
        name.Should().Be("James".ToValue());
    }

    [Test]
    public void WhenTryingToGetAValueWithAKeyThatDoesNotExistsFalseIsReturnedAndTheValueIsSetToEmpty()
    {
        var dict = new DictionaryValue(
            "name".ToValue(), "James".ToValue(),
            "age".ToValue(), "38".ToValue());

        var success = dict.TryGetValue("unknown".ToValue(), out var name);

        success.Should().BeFalse();
        name.Should().Be(Value.Empty);
    }

    [Test]
    public void ADictionaryValueReturnsItselfWhenAskedToConvertToADictionary()
    {
        Value value = new DictionaryValue();

        var dict = value.ToDictionaryValue();

        dict.Should().BeSameAs(value);
    }

    [TestCase("name", true)]
    [TestCase("height", false)]
    public void TheDictionaryValueCanBeQueriedForAKeysExistence(string key, bool expected)
    {
        var dict = new DictionaryValue(
            "name".ToValue(), "James".ToValue(),
            "age".ToValue(), "38".ToValue());

        var exists = dict.ContainsKey(key.ToValue());

        exists.Should().Be(expected);
    }

    [Test]
    public void DictionaryValuesCanBeConvertedToListValues()
    {
        var value = new DictionaryValue("a".ToValue(), "b".ToValue(), "c".ToValue());

        var list = value.ToListValue();

        ((Value)list).Should().Be(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue(), Value.Empty));
    }

    [Test]
    public void ADictionaryDoesNotConvertToANumber()
    {
        var dict1 = new DictionaryValue();
        var dict2 = new DictionaryValue(Value.Empty);
        var dict3 = new DictionaryValue(new NumberValue(1.0), " ".ToValue());

        double.IsNaN(dict1.ToDouble()).Should().BeTrue();
        double.IsNaN(dict2.ToDouble()).Should().BeTrue();
        double.IsNaN(dict3.ToDouble()).Should().BeTrue();
    }

    [Test]
    public void ADictionaryAlwaysConvertsToATrueBool()
    {
        var dict1 = new DictionaryValue();
        var dict2 = new DictionaryValue(Value.Empty);
        var dict3 = new DictionaryValue(new NumberValue(1.0), " ".ToValue());

        dict1.ToBool().Should().BeTrue();
        dict2.ToBool().Should().BeTrue();
        dict3.ToBool().Should().BeTrue();
    }

    [Test]
    public void AValueCanBeRetrievedFromADictionaryByKey()
    {
        var dict = new DictionaryValue("a".ToValue(), 1.ToValue());

        var value = dict["a".ToValue()];

        value.Should().Be(1.ToValue());
    }

    [Test]
    public void WhenRetrievingAValueFromADictionaryByAKeyTheDoesNotExistAnErrorIsThrown()
    {
        var dict = new DictionaryValue("a".ToValue(), 1.ToValue());

        // TODO:  Implement this test.
    }

    [Test]
    public void AValueCanBeSetByKeyInADictionary()
    {
        var dict1 = new DictionaryValue("a".ToValue(), 1.ToValue());

        var dict2 = dict1.SetItem("b".ToValue(), 2.ToValue());

        dict2.Should().Be(new DictionaryValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue()));
    }
}