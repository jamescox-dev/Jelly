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
        var dict = new DictionaryValue(new KeyValuePair<Value, Value>[]
        {
            new(new StringValue("zProp"), new StringValue("26")),
            new(new StringValue("aProp"), new StringValue("1")),
        });

        var str = dict.ToString();

        str.Should().Be("aProp 1 zProp 26");
    }

    [Test]
    public void AValueFromTheDictionaryCanBeRetrievedWithAKey()
    {
        var dict = new DictionaryValue(new KeyValuePair<Value, Value>[]
        {
            new("name".ToValue(), "James".ToValue()),
            new("age".ToValue(), "38".ToValue()),
        });

        var name = dict["name".ToValue()];
        var age = dict["age".ToValue()];
        
        name.Should().Be("James".ToValue());
        age.Should().Be("38".ToValue());
    }
}