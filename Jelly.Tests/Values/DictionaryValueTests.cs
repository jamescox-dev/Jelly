namespace Jelly.Values.Tests;

using Jelly.Values;

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
    public void ADictionaryValuesStringRepresentationContainsTheStringRepresentationOfEachOfItsKeysAndAssociatedValueInAlphabeticalOrderByKeySeparatedBySpaces()
    {
        var dict = new DictionaryValue(
            "zProp".ToValue(), "26".ToValue(),
            "aProp".ToValue(), "1".ToValue());

        var str = dict.ToString();

        str.Should().Be("aProp 1 zProp 26");
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
    public void ADictionaryValueReturnsItselfWhenAskedToConvertToADictionary()
    {
        Value value = new DictionaryValue();

        var dict = value.AsDictionary();

        dict.Should().BeSameAs(value);
    }
}