namespace Jelly.Serializers.Tests;

[TestFixture]
public class NodeSerializerTests
{
    [Test]
    public void AEmptyNodeIsRepresentedByTwoEmptyBrackets()
    {
        var node = new DictValue();

        var str = NodeSerializer.Serialize(node);

        str.Should().Be("[]");
    }
}