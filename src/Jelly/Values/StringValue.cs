namespace Jelly.Values;

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

    public override DictValue ToDictionaryValue()
    {
        var listParser = new ListParser();
        try
        {
            return new DictValue((IEnumerable<Value>)listParser.Parse(new Scanner(ToString())));
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