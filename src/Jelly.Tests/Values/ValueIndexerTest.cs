namespace Jelly.Tests.Values;

[TestFixture]
public class ValueIndexerTests
{
    [TestCase(1, "a")]
    [TestCase(2, "b")]
    [TestCase(3, "c")]
    [TestCase(-1, "c")]
    [TestCase(-2, "b")]
    public void AIndexedItemCanBeRetrievedFromAList(int index, string expectedValue)
    {
        var list = new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue());
        var indexer = new ListIndexer(index.ToValue());

        var item = indexer.GetItemOf(list);

        item.Should().Be(expectedValue.ToValue());
    }

    [TestCase("a", "x")]
    [TestCase("b", "y")]
    [TestCase("c", "z")]
    public void AItemCanBeRetrievedByKeyFromADict(string key, string expectedValue)
    {
        var dict = new DictValue(
            "a".ToValue(), "x".ToValue(),
            "b".ToValue(), "y".ToValue(),
            "c".ToValue(), "z".ToValue());
        var indexer = new DictIndexer(key.ToValue());

        var item = indexer.GetItemOf(dict);

        item.Should().Be(expectedValue.ToValue());
    }

    [Test]
    public void IndexersCanBeChainedTogetherToAccessItemsInNestedCollections()
    {
        var nest = new ListValue(
            "a".ToValue(),
            "b".ToValue(),
            new DictValue(
                "x".ToValue(), new ListValue(
                    "i".ToValue(), "j".ToValue(), "k".ToValue()
                )
            )
        );
        var indexer = new ListIndexer((-1).ToValue(), new DictIndexer("x".ToValue(), new ListIndexer(2.ToValue())));

        var item = indexer.GetItemOf(nest);

        item.Should().Be("j".ToValue());
    }

    [Test]
    public void AItemInAListCanBeSetByIndex()
    {
        var list = new ListValue("a".ToValue(), "y".ToValue(), "c".ToValue());
        var indexer = new ListIndexer(2.ToValue());

        var updatedList = indexer.SetItemOf(list, "b".ToValue());

        updatedList.Should().Be(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue()));
    }

    [Test]
    public void AItemInADictCanBeSetByKey()
    {
        var dict = new DictValue(
            "a".ToValue(), "1".ToValue(),
            "b".ToValue(), "y".ToValue(),
            "c".ToValue(), "3".ToValue());
        var indexer = new DictIndexer("b".ToValue());

        var updatedList = indexer.SetItemOf(dict, "2".ToValue());

        updatedList.Should().Be(new DictValue(
            "a".ToValue(), "1".ToValue(),
            "b".ToValue(), "2".ToValue(),
            "c".ToValue(), "3".ToValue()));
    }

    [Test]
    public void IndexersCanBeChainedTogetherToSetItemsInNestedCollections()
    {
        var nest = new ListValue(
            "a".ToValue(),
            new DictValue(
                "x".ToValue(), 1.ToValue(),
                "y".ToValue(), new ListValue(
                    "i".ToValue(), "j".ToValue(), "_".ToValue()
                )
            ),
            "b".ToValue()
        );
        var indexer = new ListIndexer(2.ToValue(), new DictIndexer("y".ToValue(), new ListIndexer(3.ToValue())));

        var newNest = indexer.SetItemOf(nest, "k".ToValue());

        newNest.Should().Be(new ListValue(
            "a".ToValue(),
            new DictValue(
                "x".ToValue(), 1.ToValue(),
                "y".ToValue(), new ListValue(
                    "i".ToValue(), "j".ToValue(), "k".ToValue()
                )
            ),
            "b".ToValue()
        ));
    }

    [Test]
    public void AListIndexerCanBeConstructedFromASingleItemListOfIndexerNodes()
    {
        var env = new Env();
        var indexers = new ListValue(Node.ListIndexer(0, 0, Node.Literal(2)));

        var indexer = ValueIndexer.FromIndexerNodes(indexers, env);

        indexer.Should().Be(new ListIndexer(2.ToValue()));
    }

    [Test]
    public void ADictIndexerCanBeConstructedFromASingleItemListOfIndexerNodes()
    {
        var env = new Env();
        var indexers = new ListValue(Node.DictIndexer(0, 0, Node.Literal("a")));

        var indexer = ValueIndexer.FromIndexerNodes(indexers, env);

        indexer.Should().Be(new DictIndexer("a".ToValue()));
    }

    [Test]
    public void WhenTheIndexerIsOfAnUnknownTypeAnExceptionIsThrown()
    {
        var env = new Env();
        var indexers = new ListValue(new DictValue(
            Keywords.Type, "odd".ToValue(),
            Keywords.Expression, Node.Literal(1)
        ));

        var action = () => ValueIndexer.FromIndexerNodes(indexers, env);

        action.Should().Throw<ParseError>().WithMessage("Unknown indexer type 'odd'.");
    }

    [Test]
    public void WhenTheIndexerIsMissingATypeAnExceptionIsThrown()
    {
        var env = new Env();
        var indexers = new ListValue(new DictValue(
            Keywords.Expression, Node.Literal(1)
        ));

        var action = () => ValueIndexer.FromIndexerNodes(indexers, env);

        action.Should().Throw<ParseError>().WithMessage("Invalid indexer node.");
    }

    [Test]
    public void WhenTheIndexerIsMissingAnExpressionAnExceptionIsThrown()
    {
        var env = new Env();
        var indexers = new ListValue(new DictValue(
            Keywords.Type, Keywords.ListIndexer
        ));

        var action = () => ValueIndexer.FromIndexerNodes(indexers, env);

        action.Should().Throw<ParseError>().WithMessage("Invalid indexer node.");
    }

    [Test]
    public void AMultiItemListOfIndexerNodesResultsInAChainedIndexer()
    {
        var env = new Env();
        var indexers = new ListValue(
            Node.ListIndexer(0, 0, Node.Literal(2)),
            Node.DictIndexer(0, 0, Node.Literal("a")));

        var indexer = ValueIndexer.FromIndexerNodes(indexers, env);

        indexer.Should().Be(new ListIndexer(2.ToValue(), new DictIndexer("a".ToValue())));
    }

    [Test]
    public void AnEmptyListOfIndexerNodesThrowsAnException()
    {
        var action = () => ValueIndexer.FromIndexerNodes(ListValue.EmptyList, null!);

        action.Should().Throw<ParseError>().WithMessage("No indexers in list.");
    }
}