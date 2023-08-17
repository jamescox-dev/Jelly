namespace Jelly.Values.Tests;

[TestFixture]
public class NumValueTests
{
    [TestCase(0.0)]
    [TestCase(0.84551240822557006)]
    [TestCase(Math.PI)]
    [TestCase(double.PositiveInfinity)]
    [TestCase(double.NegativeInfinity)]
    public void ANumberValuesStringRepresentationCanBeUsedToConstructAnIdenticalNumberValue(double number)
    {
        var num = new NumValue(number);

        var str = num.ToString();

        str.ToValue().ToDouble().Should().Be(number);
    }

    [TestCase(double.NaN)]
    public void ANumbersStringRepresentationShouldBeInLowerCase(double value)
    {
        var num = new NumValue(value);

        var str = num.ToString();

        str.Should().Be(str.ToLowerInvariant());
    }


    [TestCase("this is not a number")]
    [TestCase("_0")]
    [TestCase("0_")]
    [TestCase("_0_")]
    [TestCase("._0")]
    [TestCase(".0_")]
    [TestCase("._0_")]
    [TestCase(".0e_1")]
    [TestCase(".0e1_")]
    [TestCase(".0e_1_")]
    [TestCase("0x_1")]
    [TestCase("0x1_")]
    [TestCase("0x_1_")]
    [TestCase("0o_1")]
    [TestCase("0o1_")]
    [TestCase("0o_1_")]
    [TestCase("0b_1")]
    [TestCase("0b1_")]
    [TestCase("0b_1_")]
    [TestCase(".")]
    [TestCase("e")]
    public void ParsingANumberResultsInANaNWhenTheStringIsNotInAValidFormat(string numberString)
    {
        var number = NumValue.ParseNumber(numberString);

        double.IsNaN(number).Should().BeTrue();
    }

    [TestCase("true", 1.0)]
    [TestCase("True", 1.0)]
    [TestCase("TRUE", 1.0)]
    [TestCase("false", 0.0)]
    [TestCase("FALSE", 0.0)]
    [TestCase("fAlSe", 0.0)]
    [TestCase("inf", double.PositiveInfinity)]
    [TestCase("INF", double.PositiveInfinity)]
    [TestCase("inF", double.PositiveInfinity)]
    [TestCase("-inf", double.NegativeInfinity)]
    [TestCase("-Inf", double.NegativeInfinity)]
    [TestCase("-InF", double.NegativeInfinity)]
    public void SpecialWordsAreParsedIntoThereCorrectValue(string numberString, double expected)
    {
        var number = NumValue.ParseNumber(numberString);

        number.Should().Be(expected);
    }

    [TestCase("  true", 1.0)]
    [TestCase("  inf  \n\t", double.PositiveInfinity)]
    [TestCase("false\t\t", 0.0)]
    public void LeadingAndTrailingSpacesAreIgnoredWhenParsingANumber(string numberString, double expected)
    {
        var number = NumValue.ParseNumber(numberString);

        number.Should().Be(expected);
    }

    [TestCase("0", 0.0)]
    [TestCase("42", 42.0)]
    [TestCase("-42", -42.0)]
    [TestCase("+10", 10.0)]
    [TestCase("810038390800", 810038390800.0)]
    [TestCase("100000000000000000000", 100000000000000000000.0)]
    [TestCase("-99999999999999999999999", -99999999999999999999999.0)]
    public void IntegerCanBeParsed(string numberString, double expected)
    {
        var number = NumValue.ParseNumber(numberString);

        number.Should().Be(expected);
    }

    [TestCase("0.05", 0.05)]
    [TestCase("+1.876e10", 1.876e10)]
    [TestCase("-677e28", -677e28)]
    [TestCase("-0.05", -0.05)]
    [TestCase("1.98676876876876", 1.98676876876876)]
    public void DecimalNumbersCanBeParsed(string numberString, double expected)
    {
        var number = NumValue.ParseNumber(numberString);

        number.Should().Be(expected);
    }

    [TestCase("0xff", 255)]
    [TestCase("0x100", 256)]
    [TestCase("0x100000000000", 0x100000000000)]
    [TestCase("0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", double.MaxValue)]
    [TestCase("-0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", -double.MaxValue)]
    [TestCase("0o177777777777777777400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", double.MaxValue)]
    [TestCase("-0o177777777777777777400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", -double.MaxValue)]
    [TestCase("-0xf", -15)]
    public void HexadecimalNumbersCanBeParsed(string numberString, double expected)
    {
        var number = NumValue.ParseNumber(numberString);

        number.Should().Be(expected);
    }

    [TestCase("0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", 1)]
    [TestCase("-0xfffffffffffff800000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", -1)]
    [TestCase("0o177777777777777777400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", 1)]
    [TestCase("-0o177777777777777777400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", -1)]
    [TestCase("1.7976931348623159e308", 1)]
    [TestCase("-1.7976931348623159e308", -1)]
    public void WhenTheNumberIsOutOfTheRangeOfADoubleFloatAInfinityIsReturned(string numberString, int expectedSign)
    {
        var number = NumValue.ParseNumber(numberString);

        Math.Sign(number).Should().Be(expectedSign);
        double.IsInfinity(number).Should().BeTrue();
    }

    [TestCase("1_000_000", 1_000_000.0)]
    [TestCase("3.141_593", 3.141_593)]
    [TestCase("1_234.567_890", 1_234.567_890)]
    [TestCase("0.123_456", 0.123_456)]
    [TestCase("1_000_000e1_0", 1_000_000e1_0)]
    [TestCase("3.141_593e1_1", 3.141_593e1_1)]
    [TestCase("1_234.567_890e0_1", 1_234.567_890e0_1)]
    [TestCase("0.123_456e4_2", 0.123_456e4_2)]
    [TestCase("0xffff_ffff", 0xffff_ffff)]
    public void UnderscoresAreIgnoredBetweenGroupsOfDigits(string numberString, double expected)
    {
        var number = NumValue.ParseNumber(numberString);

        number.Should().Be(expected);
    }

    [TestCase("0b11", 3)]
    [TestCase("0b100", 4)]
    [TestCase("0b1000_0000_0000", 0b100000000000)]
    [TestCase("-0b1", -1)]
    public void BinaryNumbersCanBeParsed(string numberString, double expected)
    {
        var number = NumValue.ParseNumber(numberString);

        number.Should().Be(expected);
    }

    [TestCase("0o77", 63)]
    [TestCase("0o100", 64)]
    [TestCase("0o300_200_100", 50397248)]
    [TestCase("-0o7", -7)]
    public void OctalNumbersCanBeParsed(string numberString, double expected)
    {
        var number = NumValue.ParseNumber(numberString);

        number.Should().Be(expected);
    }

    [TestCase(double.NaN, true)]
    [TestCase(3.142, true)]
    [TestCase(double.PositiveInfinity, true)]
    [TestCase(double.NegativeInfinity, true)]
    [TestCase(1.0, true)]
    [TestCase(-1.0, true)]
    [TestCase(0.0, false)]
    public void ANumberCanBeConvertedToABool(double d, bool expected)
    {
        var number = new NumValue(d);

        var b = number.ToBool();

        b.Should().Be(expected);
    }

    [Test]
    public void ANumberConvertsToAListWithTheNumberAsItsOnlyItem()
    {
        var num = new NumValue(42);

        var list = num.ToListValue();

        list.Single().Should().Be(num);
    }

    [Test]
    public void ANumberIsConvertedToADictionaryWithItsFirstItemWithTheNumberAsItsKeyAndAnEmptyValue()
    {
        var num = new NumValue(42);

        var dict = num.ToDictValue();

        dict.Should().Be(new DictValue(num, Value.Empty));
    }
}