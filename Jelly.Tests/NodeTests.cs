namespace Jelly.Tests;

using Jelly;

[TestFixture]
public class NodeTests
{
    [Test]
    public void ANodesStringRepresentationBeginsWithItsType()
    {
        var node = new Node("literal");

        var str = node.ToString();

        str.Should().StartWith("literal");
    }

    [Test]
    public void ANodesStringRepresentationContainsAKeyValuePairForEachPropertyOfTheNodeInAlphabeticalOrder()
    {
        var node = new Node("node", new SortedDictionary<string, Value>
        {
            { "zProp", new StringValue("26") },
            { "aProp", new StringValue("1") },
        });

        var str = node.ToString();

        str.Should().Be("node aProp 1 zProp 26");
    }

    [Test]
    public void WhenANodeHasNoPropertiesItsStringRepresentationIsJustItsTypeName()
    {
        var node = new Node("me");

        var str = node.ToString();

        str.Should().Be("me");
    }
}