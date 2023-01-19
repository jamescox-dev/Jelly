namespace Jelly.Serializers.Tests;

using Jelly.Serializers;
using Jelly.Values;

[TestFixture]
public class ValueSerializerTests
{
    [Test]
    public void AnEmptyValueIsSerializedAsTwoSingleQuotes()
    {
        var str = ValueSerializer.Serialize(Value.Empty);

        str.Should().Be("''");
    }

    [Test]
    public void AValueThatDoesNotContainSpecialCharactersIsReturnedAsIs()
    {
        var str = ValueSerializer.Serialize("hello".ToValue());

        str.Should().Be("hello");
    }
}