namespace Jelly.Serializers.Tests;

[TestFixture]
public class NodeSerializerTests
{
    [Test]
    public void AEmptyNodeIsRepresentedByTwoEmptyBrackets()
    {
        var node = new DictionaryValue();

        var str = NodeSerializer.Serialize(node);

        str.Should().Be("[]");
    }
}