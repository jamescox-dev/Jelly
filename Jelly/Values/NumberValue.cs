namespace Jelly.Values;

using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

public class NumberValue : Value
{
    public static readonly NumberValue One = new NumberValue(1.0);

    static readonly BigInteger DoubleMaxValueAsBigInt = new BigInteger(double.MaxValue);
    static readonly Regex NumberPattern = new Regex(@"
        ^\s*
        (
            [+-]?
            (
                (
                    ([0-9]|[0-9][0-9_]*?[0-9])
                    (
                        \.
                    |
                        \.([0-9]|[0-9][0-9_]*?[0-9])
                    )?
                )
                (e[+-]?([0-9]|[0-9][0-9_]*?[0-9]))?
            |
                (0x([0-9a-f]|[0-9a-f][0-9a-f_]*?[0-9a-f]))
            |
                (0o([0-7]|[0-7][0-7_]*?[0-7]))
            |
                (0b([01]|[01][01_]*?[01]))
            |
                inf
            )
        |
            true|false
        )
        \s*$
    ", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

    static readonly Dictionary<string, double> SpecialWords = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "true", 1.0 }, { "false", 0.0 }, { "inf", double.PositiveInfinity }, { "-inf", double.NegativeInfinity }
    };

    readonly double _value;
    
    public NumberValue(double value)
    {
        _value = value;
    }

    public NumberValue(string value)
    {
        _value = double.Parse(value, NumberStyles.AllowLeadingWhite, CultureInfo.InvariantCulture);
    }

    public override ListValue ToListValue() => new ListValue(this);

    public override DictionaryValue ToDictionaryValue() => new DictionaryValue(this);

    public override bool ToBool() => double.IsNaN(_value) || _value != 0.0;

    public override double ToDouble() => _value;

    public override string ToString()
    {
        return _value.ToString("G17");
    }

    public static double ParseNumber(string number)
    {
        if (ValidateAndFixNumber(ref number))
        {
            if (TryParseDecimal(number, out var parsedDecimal))
            {
                return parsedDecimal;
            }
            if (TryParseHexademimal(number, out var parsedHexadecimal))
            {
                return parsedHexadecimal;
            }
            if (TryParseSpecialWord(number, out var parsedWordValue))
            {
                return parsedWordValue;
            }
            if (TryParseBinary(number, out var parsedBinary))
            {
                return parsedBinary;
            }
            if (TryParseOctal(number, out var parsedOctal))
            {
                return parsedOctal;
            }
        }
        return double.NaN;
    }

    public static bool IsValidNumber(string number)
    {
        return NumberPattern.IsMatch(number);
    }

    static bool ValidateAndFixNumber(ref string number)
    {
        if (IsValidNumber(number))
        {
            number = number.Trim().Replace("_", "");
            return true;
        }
        return false;
    }

    static bool TryParseSpecialWord(string number, out double value)
    {
        return SpecialWords.TryGetValue(number, out value);
    }

    static bool TryParseDecimal(string number, out double value)
    {
        return double.TryParse(number, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out value);
    }

    static bool TryParseHexademimal(string number, out double value)
    {
        var sign = 1;
        if (number.StartsWith('-'))
        {
            number = number.Substring(1);
            sign = -1;
        }
        if (number.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
        {
            number = $"0{number.Substring(2)}";
            if (BigInteger.TryParse(number, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var parsedBigInt))
            {
                if (parsedBigInt > DoubleMaxValueAsBigInt)
                {
                    value = double.PositiveInfinity * sign;     
                }
                else
                {
                    value = ((double)parsedBigInt) * sign;
                }
                return true;
            }
        }

        value = 0.0;
        return false;
    }

    static bool TryParseBinary(string number, out double value)
    {
        var sign = 1;
        if (number.StartsWith('-'))
        {
            number = number.Substring(1);
            sign = -1;
        }
        if (number.StartsWith("0b", StringComparison.CurrentCultureIgnoreCase))
        {
            number = number.Substring(2);
            var parsedBigInt = BigInteger.Zero;
            var postionValue = BigInteger.One;
            foreach (var ch in number.Reverse())
            {
                if (ch == '1')
                {
                    parsedBigInt += postionValue;
                }
                else if (ch != '0')
                {
                    value = 0.0;
                    return false;
                }
                postionValue <<= 1;
            }
            if (parsedBigInt > DoubleMaxValueAsBigInt)
            {
                value = double.PositiveInfinity * sign;     
            }
            else
            {
                value = ((double)parsedBigInt) * sign;
            }
            return true;
        }

        value = 0.0;
        return false;
    }

    static bool TryParseOctal(string number, out double value)
    {
        var sign = 1;
        if (number.StartsWith('-'))
        {
            number = number.Substring(1);
            sign = -1;
        }
        if (number.StartsWith("0o", StringComparison.CurrentCultureIgnoreCase))
        {
            number = number.Substring(2);
            var parsedBigInt = BigInteger.Zero;
            var postionValue = BigInteger.One;
            foreach (var ch in number.Reverse())
            {
                if (ch >= '0' && ch <= '7')
                {
                    parsedBigInt += postionValue * (ch - '0');
                }
                else
                {
                    value = 0.0;
                    return false;
                }
                postionValue <<= 3;
            }
            if (parsedBigInt > DoubleMaxValueAsBigInt)
            {
                value = double.PositiveInfinity * sign;     
            }
            else
            {
                value = ((double)parsedBigInt) * sign;
            }
            return true;
        }

        value = 0.0;
        return false;
    }
}