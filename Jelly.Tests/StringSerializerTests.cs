namespace Jelly.Tests;

using Jelly;

[TestFixture]
public class StringSerializerTests
{
    [Test]
    public void AnEmptyStringIsSerializedAsTwoSingleQuotes()
    {
        var str = StringSerializer.Serialize(string.Empty);

        str.Should().Be("''");
    }

    [Test]
    public void AStringThatDoesNotContainSpecialCharactersIsReturnedAsIs()
    {
        var str = StringSerializer.Serialize("hello");

        str.Should().Be("hello");
    }
}