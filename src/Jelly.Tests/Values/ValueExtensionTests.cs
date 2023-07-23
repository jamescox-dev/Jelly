namespace Jelly.Values.Tests;

[TestFixture]
public class ValueExtensionsTests
{
    ListValue testList = null!;

    [SetUp]
    public void Setup()
    {
        testList = new ListValue("x".ToValue(), "y".ToValue(), "z".ToValue());
    }

    [Test]
    public void ConvertingAValueFromAOneBasedIndexToANativeIndexResultsInAZeroBasedIndex()
    {
        var jellyIndex = new NumValue(2);

        var index = jellyIndex.ToIndexOf(testList);

        index.Should().Be(1);
    }

    [Test]
    public void ConvertingAValueFromANegativeOneBasedIndex_OrReverseIndex_ToANativeIndexResultsInAZeroBasedIndex()
    {
        var jellyIndex = new NumValue(-1);

        var index = jellyIndex.ToIndexOf(testList);

        index.Should().Be(2);
    }

    [TestCase(-2.75, 1)]
    [TestCase(-2.25, 1)]
    [TestCase(-1.75, 2)]
    [TestCase(-1.25, 2)]
    [TestCase(1.25, 0)]
    [TestCase(1.75, 0)]
    [TestCase(2.25, 1)]
    [TestCase(2.75, 1)]
    public void IndexesWithDecimalDigitsAreTruncated(double jellyIndexDouble, int expectedIndex)
    {
        var jellyIndex = new NumValue(jellyIndexDouble);

        var index = jellyIndex.ToIndexOf(testList);

        index.Should().Be(expectedIndex);
    }

    [Test]
    public void ZeroIndexesCanNotBeConvertedToIndexes()
    {
        var jellyIndex = NumValue.Zero;

        jellyIndex.Invoking(i => i.ToIndexOf(testList))
            .Should().Throw<ValueError>().WithMessage("index must not be zero.");
    }

    [TestCase(double.NegativeInfinity)]
    [TestCase(double.PositiveInfinity)]
    [TestCase(double.NaN)]
    public void InfiniteOrNonNumericValuesCanNotBeConvertedToIndexes(double jellyIndexDouble)
    {
        var jellyIndex = new NumValue(jellyIndexDouble);

        jellyIndex.Invoking(i => i.ToIndexOf(testList))
            .Should().Throw<ValueError>().WithMessage("index must be a finite number.");
    }
}