namespace Jelly.Values;

using Jelly.Errors;
using Jelly.Parser;
using Jelly.Parser.Scanning;

public class StringValue : Value
{
    readonly string _value;

    public StringValue(string value)
    {
        _value = value;
    }

    public override ListValue ToListValue()
    {
        var listParser = new ListParser();
        try
        {
            return listParser.Parse(new Scanner(ToString()));
        }
        catch (ParseError)
        {
            throw new TypeError("Value is not a list.");
        }
    }

    public override DictionaryValue ToDictionaryValue()
    {
        var listParser = new ListParser();
        try
        {
            return new DictionaryValue((IEnumerable<Value>)listParser.Parse(new Scanner(ToString())));
        }
        catch (ParseError)
        {
            throw new TypeError("Value is not a dictionary.");
        }
    }

    public override string ToString() => _value;

    public override bool ToBool()
    {
        var num = ToDouble();

        return double.IsNaN(num) || num != 0.0;
    }

    public override double ToDouble() => NumberValue.ParseNumber(ToString());
}