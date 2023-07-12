namespace Jelly.Values.Tests;

[TestFixture]
public class BooleanValueTests
{
    [TestCase(true, "true")]
    [TestCase(false, "false")]
    public void ABooleanValueCanBeCreatedForTrueAndFalseWithTheCorrectStringRepresentation(bool b, string expectedStr)
    {
        var boolValue = b.ToValue();

        var str = boolValue.ToString();

        str.Should().Be(expectedStr);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ABooleanValueCanBeConvertedToANativeBool(bool b1)
    {
        var boolValue = b1.ToValue();

        var b2 = boolValue.ToBool();

        b2.Should().Be(b1);
    }

    [TestCase(true, 1.0)]
    [TestCase(false, 0.0)]
    public void ABooleanValueCanBeConvertedToANativeDouble(bool b, double expectedDouble)
    {
        var boolValue = b.ToValue();

        var d = boolValue.ToDouble();

        d.Should().Be(expectedDouble);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ABooleanValueCanBeConvertedToList(bool b)
    {
        var boolValue = b.ToValue();

        var list = boolValue.ToListValue();

        ((IEnumerable<Value>)list).Single().Should().Be(boolValue);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ABooleanValueCanBeConvertedToDictionary(bool b)
    {
        var boolValue = b.ToValue();

        var dict = boolValue.ToDictionaryValue();

        dict[boolValue].Should().Be(Value.Empty);
    }
}