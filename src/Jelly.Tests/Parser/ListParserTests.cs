namespace Jelly.Parser.Tests;

[TestFixture]
public class ListParserTests
{
    ListParser parser = null!;

    [TestCase("")]
    [TestCase("  ")]
    [TestCase("\t\r\n")]
    public void AEmptyStringOrListItemSeparatorOnlyStringIsPasredToAnEmptyList(string listString)
    {
        var list = parser.Parse(new Scanner(listString));

        ((IEnumerable<Value>)list).Should().BeEmpty();
    }

    [TestCase("a b")]
    [TestCase("\ta\t\nb")]
    [TestCase("\n\r\ta     b")]
    public void LeadingListItemSeparatorsAreSkippedBeforeParsingEachItem(string listString)
    {
        var list = parser.Parse(new Scanner(listString));

        ((Value)list).Should().Be(new ListValue("a".ToValue(), "b".ToValue()));
    }

    [Test]
    public void TrailingListItemSeparatorsAreIgnored()
    {
        var list = parser.Parse(new Scanner("a\t\n\r   "));

        ((Value)list).Should().Be(new ListValue("a".ToValue()));
    }

    [TestCase("[")]
    [TestCase("{")]
    public void IfAItemCanNotBeParsedAErrorIsThrown(string listString)
    {
        parser.Invoking(p => p.Parse(new Scanner(listString))).Should().Throw<ParseError>();
    }

    [SetUp]
    public void Setup()
    {
        parser = new ListParser();
    }
}