namespace Jelly.Tests.Values;

[TestFixture]
public class TypeMarshallerTests
{
    TypeMarshaller _marshaller = null!;

    [SetUp]
    public void Setup()
    {
        _marshaller = new TypeMarshaller();
    }

    [Test]
    public void JellyValuesAreMarshalledToJellyValuesBySimplyReturningTheInstancePassedIn()
    {
        var jellyValueIn = "test".ToValue();
        var jellyBoolIn = BoolValue.True;
        var jellyNumIn = NumValue.One;
        var jellyStrIn = "Jelly".ToValue();
        var jellyListIn = new ListValue();
        var jellyDictIn = new DictValue();

        var jellyValueOut = _marshaller.Marshal(jellyValueIn, typeof(Value));
        var jellyBoolOut = _marshaller.Marshal((object?)jellyBoolIn);
        var jellyNumOut = _marshaller.Marshal((object?)jellyNumIn);
        var jellyStrOut = _marshaller.Marshal((object?)jellyStrIn);
        var jellyListOut = _marshaller.Marshal((object?)jellyListIn);
        var jellyDictOut = _marshaller.Marshal((object?)jellyDictIn);

        jellyValueOut.Should().BeSameAs(jellyValueIn);
        jellyBoolOut.Should().BeSameAs(jellyBoolIn);
        jellyNumOut.Should().BeSameAs(jellyNumIn);
        jellyStrOut.Should().BeSameAs(jellyStrIn);
        jellyListOut.Should().BeSameAs(jellyListIn);
        jellyDictOut.Should().BeSameAs(jellyDictIn);
    }

    [TestCase("0", typeof(BoolValue), "false")]
    [TestCase("1", typeof(BoolValue), "true")]
    [TestCase("42", typeof(NumValue), "42")]
    [TestCase("Jelly", typeof(StrValue), "Jelly")]
    [TestCase("1 2", typeof(ListValue), "1\n2")]
    [TestCase("a b", typeof(DictValue), "a b")]
    public void JellyValuesCanBeExplicitlyConvertedIntoJellyValuesByReturningTheInstancePassedIn(string jellyValueString, Type jellyType, string expectedJellyValue)
    {
        var jellyValue = _marshaller.Marshal(jellyValueString.ToValue(), jellyType);

        jellyValue.GetType().Should().Be(jellyType);
        jellyValue.ToString().Should().Be(expectedJellyValue);
    }

    [TestCase(null, typeof(StrValue), "")]
    [TestCase(true, typeof(BoolValue), "true")]
    [TestCase(false, typeof(BoolValue), "false")]
    [TestCase(-1, typeof(NumValue), "-1")]
    [TestCase(0, typeof(NumValue), "0")]
    [TestCase(1, typeof(NumValue), "1")]
    [TestCase(double.NegativeInfinity, typeof(NumValue), "-inf")]
    [TestCase(double.NaN, typeof(NumValue), "nan")]
    [TestCase(double.PositiveInfinity, typeof(NumValue), "inf")]
    [TestCase(3.25, typeof(NumValue), "3.25")]
    [TestCase("jello", typeof(StrValue), "jello")]
    public void SimpleClrTypesMarshalToTheExpectedJellyValueAndTypes(object? clrValue, Type expectedJellyType, string expectedJellyValue)
    {
        var jellyValue = _marshaller.Marshal(clrValue);

        jellyValue.GetType().Should().Be(expectedJellyType);
        jellyValue.ToString().Should().Be(expectedJellyValue);
    }

    [Test]
    public void IfAClrTypeIsEnumerableAJellyListValueIsReturnedWithEachOfTheEnumeratedItemsMarshalledToJellyValues()
    {
        var clrValue = new object?[] { null, 1, true, 1.0, "hi", new object[] { "bye" } };

        var jellyValue = _marshaller.Marshal(clrValue);

        jellyValue.GetType().Should().Be(typeof(ListValue));
        jellyValue.Should().Be(new ListValue(
            Value.Empty,
            NumValue.One,
            BoolValue.True,
            NumValue.One,
            "hi".ToValue(),
            new ListValue("bye".ToValue())));
    }

    [Test]
    public void IfAClrTypeIsADictionaryAJellyDictionaryValueIsReturnedWithEachOfItsKeyValueParisMarshalledToJellyValues()
    {
        var clrValue = new Dictionary<object, object?>
        {
            { 1, 2.0 },
            { true, "false" },
            { "this", new object[] { "that" } }
        };

        var jellyValue = _marshaller.Marshal(clrValue);

        jellyValue.GetType().Should().Be(typeof(DictValue));
        jellyValue.Should().Be(new DictValue(
            NumValue.One, 2.0.ToValue(),
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
        var b = _marshaller.Marshal(new StrValue(jellyValue), typeof(bool));

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
        var i = _marshaller.Marshal(new StrValue(jellyValue), typeof(int));

        i.GetType().Should().Be(typeof(int));
        i.Should().Be(expectedInt32);
    }

    [TestCase("nan")]
    [TestCase("-inf")]
    [TestCase("inf")]
    public void NaNOrInfinityJellyValuesThrowExceptionsWhenMarshalledToClrInt32s(string jellyValue)
    {
        _marshaller.Invoking(m => m.Marshal(new StrValue(jellyValue), typeof(int)))
            .Should().Throw<TypeError>("Invalid integer.");
    }

    [TestCase("0", 0.0)]
    [TestCase("2.5", 2.5)]
    [TestCase("inf", double.PositiveInfinity)]
    [TestCase("nan", double.NaN)]
    [TestCase("-inf", double.NegativeInfinity)]
    public void JellyValuesCanBeMarshalledToClrDoubles(string jellyValue, double expectedDouble)
    {
        var d = _marshaller.Marshal(new StrValue(jellyValue), typeof(double));

        d.GetType().Should().Be(typeof(double));
        d.Should().Be(expectedDouble);
    }

    [Test]
    public void JellyValuesCanBeMarshalledToClrString()
    {
        var jellyValue = new StrValue("hello");

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
}