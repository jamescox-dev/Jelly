namespace Jelly.Tests.Values;

public class ValueTests
{
  [TestCaseSource(nameof(AllValuesAreStringsTestCaseData))]
  public void AllValuesAreStrings(Value value, string expectedString)
  {
    var str = value.ToString();
    var jellyStr = value.ToStrValue();

    str.Should().Be(expectedString);
    jellyStr.Should().Be(expectedString.ToValue());
  }

  static readonly IReadOnlyList<TestCaseData> AllValuesAreStringsTestCaseData = new List<TestCaseData>
    {
        new(new StrValue("hello, world"), "hello, world"),
    };

  [TestCase("a", "A", 0)]
  [TestCase("Hello", "Hi", -1)]
  [TestCase("same", "SAME", 0)]
  [TestCase("Hi", "bye", 1)]
  public void ValuesAreComparedAsStringsWithCaseInsensitiveInvariantCulture(string str1, string str2, int expected)
  {
    var value1 = str1.ToValue();
    var value2 = str2.ToValue();

    var result = value1.CompareTo(value2);

    result.Should().Be(expected);
  }

  [TestCase("one", "two")]
  [TestCase("three", "four")]
  [TestCase("five", "five")]
  public void ValuesAreEqualIfTheirComparisonToAnotherValueIsZero(string str1, string str2)
  {
    var value1 = str1.ToValue();
    var value2 = str2.ToValue();
    var comparison = value1.CompareTo(value2);

    var equals = value1.Equals(value2);

    equals.Should().Be(comparison == 0);
  }

  [Test]
  public void ValuesComparedForEqualityAgainstNonValueTypesAreNotEqual()
  {
    var value = "0".ToValue();

    value.Equals(0).Should().BeFalse();
    value.Equals("0").Should().BeFalse();
    value.Equals(null).Should().BeFalse();
  }

  [Test]
  public void WhenValuesComparedForEqualityAreBothValueTypesTheValueSpecificEqualsMethodIsUsed()
  {
    var value1 = "test".ToValue();
    object value2 = "test".ToValue();

    var equal = value1.Equals(value2);

    equal.Should().BeTrue();
  }

  [Test]
  public void WhenAValueIsComparedToANullValueTheNullValueIsTreatedAsAnEmptyString()
  {
    var empty = Value.Empty;

    var comparison = empty.CompareTo(null);

    comparison.Should().Be(0);
  }

  [TestCase("Homer")]
  [TestCase("Marge")]
  [TestCase("Bart")]
  [TestCase("Lisa")]
  [TestCase("Maggie")]
  public void ValuesHashCodesAreCalculatedWithTheInvariantCultureCaseInsensitive(string str)
  {
    var value = str.ToValue();

    var hash = value.GetHashCode();

    hash.Should().Be(StringComparer.InvariantCultureIgnoreCase.GetHashCode(str));
  }

  [Test]
  public void ValueProvidesAConstantEmptyValue()
  {
    Value.Empty.ToString().Should().Be(string.Empty);
  }

  [TestCase("0", false)]
  [TestCase("1", true)]
  [TestCase("-10000", true)]
  [TestCase("false", false)]
  public void ValuesCanBeConvertedParsedIntoBools(string str, bool expected)
  {
    var value = str.ToValue();

    var clrBool = value.ToBool();
    var boolValue = value.ToBoolValue();

    clrBool.Should().Be(expected);
    boolValue.Should().Be(expected.ToValue());
  }

  [TestCase("0", 0)]
  [TestCase("1", 1)]
  [TestCase("+2", 2)]
  [TestCase("-10000", -10000)]
  [TestCase("bob", double.NaN)]
  public void ValuesCanBeConvertedParsedIntoDoubles(string str, double expected)
  {
    var value = str.ToValue();

    var d = value.ToDouble();
    var num = value.ToNumValue();

    if (double.IsNaN(expected))
    {
      double.IsNaN(d).Should().BeTrue();
      num.Should().Be(NumValue.NaN);
    }
    else
    {
      d.Should().Be(expected);
      num.Should().Be(expected.ToValue());
    }
  }

  [TestCase(@"Jelly")]
  [TestCase(@"jello, world")]
  [TestCase(@"'hi'")]
  [TestCase("\"bye\"")]
  [TestCase("['single' \"double\"]")]
  public void ValuesCanBeEscapedSoThatTheirValueCanBeReinterpretedByWordParserAndEvaluateBackToTheSameValue(string stringValue)
  {
    var parser = new WordParser();
    var env = new Jelly.Runtime.Env();
    var value = stringValue.ToValue();

    var escapedValue = value.Escape();

    env.Evaluate(parser.Parse(new Scanner(escapedValue))!).Should().Be(value, $"escapedValue = {escapedValue}");
  }

  [Test]
  public void ValuesCanBeConvertedToListValues()
  {
    var value = new StrValue("a b c");

    var list = value.ToListValue();

    ((Value)list).Should().Be(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue()));
  }

  [Test]
  public void IfTheValueCanNotBeParsedIntoAListValueAnTypeErrorIsThrown()
  {
    var value = new StrValue("{");

    value.Invoking(v => v.ToListValue()).Should().Throw<TypeError>().WithMessage("Value is not a list.");
  }

  [Test]
  public void ValuesCanBeConvertedToDictionaryValues()
  {
    var value = new StrValue("a b c d");

    var dict = value.ToDictValue();

    ((Value)dict).Should().Be(new DictValue("a".ToValue(), "b".ToValue(), "c".ToValue(), "d".ToValue()));
  }

  [Test]
  public void ValuesCanBeConvertedToDictionaryValueIfTheLastValueIsMissingItIsLeftEmpty()
  {
    var value = new StrValue("a b c");

    var dict = value.ToDictValue();

    ((Value)dict).Should().Be(new DictValue("a".ToValue(), "b".ToValue(), "c".ToValue(), Value.Empty));
  }

  [Test]
  public void IfTheValueCanNotBeParsedIntoADictionaryValueAnTypeErrorIsThrown()
  {
    var value = new StrValue("{");

    value.Invoking(v => v.ToDictValue()).Should().Throw<TypeError>().WithMessage("Value is not a dictionary.");
  }
}
