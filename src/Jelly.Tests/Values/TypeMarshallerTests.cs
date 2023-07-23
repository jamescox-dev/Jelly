namespace Jelly.Values.Tests;

[TestFixture]
public class TypeMarshallerTests
{
    TypeMarshaller _marshaller = null!;

    [SetUp]
    public void Setup()
    {
        _marshaller = new TypeMarshaller();
    }

    [TestCase(null, typeof(StringValue), "")]
    [TestCase(true, typeof(BoolValue), "true")]
    [TestCase(false, typeof(BoolValue), "false")]
    [TestCase(-1, typeof(NumberValue), "-1")]
    [TestCase(0, typeof(NumberValue), "0")]
    [TestCase(1, typeof(NumberValue), "1")]
    [TestCase(double.NegativeInfinity, typeof(NumberValue), "-inf")]
    [TestCase(double.NaN, typeof(NumberValue), "nan")]
    [TestCase(double.PositiveInfinity, typeof(NumberValue), "inf")]
    [TestCase(3.25, typeof(NumberValue), "3.25")]
    [TestCase("jello", typeof(StringValue), "jello")]
    public void SimpleClrTypesMarshalToTheExpectedJellyValueAndTypes(object? clrValue, Type expectedJellyType, string expectedJellyValue)
    {
        var jellyValue = _marshaller.Marshal(clrValue);

        jellyValue.GetType().Should().Be(expectedJellyType);
        jellyValue.ToString().Should().Be(expectedJellyValue);
    }

    [Test]
    public void IfAClrTypeIsEnumerableAJellyListValueIsReturnedWithEachOfTheEnumeratedItemsMarshelledToJellyValues()
    {
        var clrValue = new object?[] { null, 1, true, 1.0, "hi", new object[] { "bye" } };

        var jellyValue = _marshaller.Marshal(clrValue);

        jellyValue.GetType().Should().Be(typeof(ListValue));
        jellyValue.Should().Be(new ListValue(
            Value.Empty,
            NumberValue.One,
            BoolValue.True,
            NumberValue.One,
            "hi".ToValue(),
            new ListValue("bye".ToValue())));
    }

    [Test]
    public void IfAClrTypeIsADictionaryAJellyDictionaryValueIsReturnedWithEachOfItsKeyValueParisMarshelledToJellyValues()
    {
        var clrValue = new Dictionary<object, object?>
        {
            { 1, 2.0 },
            { true, "false" },
            { "this", new object[] { "that" } }
        };

        var jellyValue = _marshaller.Marshal(clrValue);

        jellyValue.GetType().Should().Be(typeof(DictionaryValue));
        jellyValue.Should().Be(new DictionaryValue(
            NumberValue.One, 2.0.ToValue(),
            BoolValue.True, "false".ToValue(),
            "this".ToValue(), new ListValue("that".ToValue())
        ));
    }

    [Test]
    public void IfTheTypeIsNotSupportedAnErrorIsThrown()
    {
        var clrValue = Console.In;

        _marshaller.Invoking(m => m.Marshal(clrValue)).Should()
            .Throw<TypeError>().WithMessage("Unsupported CLR type.");
    }

    [TestCase("true", true)]
    [TestCase("false", false)]
    [TestCase("1", true)]
    [TestCase("", true)]
    public void JellyValuesCanBeMarshalledToClrBools(string jellyValue, bool expectedBool)
    {
        var b = _marshaller.Marshal(new StringValue(jellyValue), typeof(bool));

        b.GetType().Should().Be(typeof(bool));
        b.Should().Be(expectedBool);
    }

    [TestCase("true", 1)]
    [TestCase("false", 0)]
    [TestCase("10000", 10000)]
    [TestCase("-1", -1)]
    [TestCase("-1000", -1000)]
    public void JellyValuesCanBeMarshalledToClrInt32s(string jellyValue, int expectedInt32)
    {
        var i = _marshaller.Marshal(new StringValue(jellyValue), typeof(int));

        i.GetType().Should().Be(typeof(int));
        i.Should().Be(expectedInt32);
    }

    [TestCase("nan")]
    [TestCase("-inf")]
    [TestCase("inf")]
    public void NaNOrInfinityJellyValuesThrowExceptionsWhenMarshalledToClrInt32s(string jellyValue)
    {
        _marshaller.Invoking(m => m.Marshal(new StringValue(jellyValue), typeof(int)))
            .Should().Throw<TypeError>("Invalid integer.");
    }

    [TestCase("0", 0.0)]
    [TestCase("2.5", 2.5)]
    [TestCase("inf", double.PositiveInfinity)]
    [TestCase("nan", double.NaN)]
    [TestCase("-inf", double.NegativeInfinity)]
    public void JellyValuesCanBeMarshalledToClrDoubles(string jellyValue, double expectedDouble)
    {
        var d = _marshaller.Marshal(new StringValue(jellyValue), typeof(double));

        d.GetType().Should().Be(typeof(double));
        d.Should().Be(expectedDouble);
    }

    [Test]
    public void JellyValuesCanBeMarshalledToClrString()
    {
        var jellyValue = new StringValue("hello");

        var s = _marshaller.Marshal(jellyValue, typeof(string));

        s.GetType().Should().Be(typeof(string));
        s.Should().Be("hello");
    }

    [Test]
    public void JellyValuesCanBeMarshalledToClrTypesThatImplementIEnumerableOfBool()
    {
        var jellyValue = new ListValue(true.ToValue(), false.ToValue());

        var s = _marshaller.Marshal(jellyValue, typeof(List<bool>));

        s.GetType().Should().Be(typeof(List<bool>));
        ((List<bool>)s).SequenceEqual(new[] { true, false }).Should().BeTrue();
    }

    [Test]
    public void JellyValuesCanBeMarshalledToClrTypesThatImplementIEnumerableOfInt32()
    {
        var jellyValue = new ListValue(1.ToValue(), 2.ToValue());

        var s = _marshaller.Marshal(jellyValue, typeof(List<int>));

        s.GetType().Should().Be(typeof(List<int>));
        ((List<int>)s).SequenceEqual(new[] { 1, 2 }).Should().BeTrue();
    }

    [Test]
    public void JellyValuesCanBeMarshalledToClrTypesThatImplementIEnumerableOfDouble()
    {
        var jellyValue = new ListValue(1.25.ToValue(), 3.5.ToValue());

        var s = _marshaller.Marshal(jellyValue, typeof(List<double>));

        s.GetType().Should().Be(typeof(List<double>));
        ((List<double>)s).SequenceEqual(new[] { 1.25, 3.5 }).Should().BeTrue();
    }

    [Test]
    public void JellyValuesCanBeMarshalledToClrTypesThatImplementIEnumerableOfString()
    {
        var jellyValue = new ListValue("a".ToValue(), "b".ToValue());

        var s = _marshaller.Marshal(jellyValue, typeof(List<string>));

        s.GetType().Should().Be(typeof(List<string>));
        ((List<string>)s).SequenceEqual(new[] { "a", "b" }).Should().BeTrue();
    }

    [Test]
    public void JellyValuesCanNotBeMarshalledToClrTypesThatImplementIEnumerableButCanBeConstructedFromOne()
    {
        var jellyValue = new ListValue(true.ToValue(), false.ToValue());

        _marshaller.Invoking(m => m.Marshal(jellyValue, typeof(bool[])))
            .Should().Throw<TypeError>("Unsupported CLR type.");
    }

    public void IfAJellyValueIsMarshalledToAnUnsuportedTypeAnErrorIsThrown()
    {
        _marshaller.Invoking(m => m.Marshal(new StringValue("hi"), typeof(FileStream))).Should()
            .Throw<TypeError>().WithMessage("Unsupported CLR type.");
    }
}