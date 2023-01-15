namespace Jelly.Tests;

using Jelly;

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
            new(new StringValue("name"), new StringValue("James")),
            new(new StringValue("age"), new StringValue("38")),
        });

        var name = dict[new StringValue("name")];
        var age = dict[new StringValue("age")];
        
        name.Should().Be(new StringValue("James"));
        name.Should().Be(new StringValue("38"));
    }
}