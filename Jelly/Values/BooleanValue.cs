namespace Jelly.Values;

public class BooleanValue : Value
{
    public static readonly BooleanValue True = new BooleanValue(true);
    public static readonly BooleanValue False = new BooleanValue(false);
    
    bool _value;

    private BooleanValue(bool value)
    {
        _value = value;
    }

    public override ListValue ToListValue() => new ListValue(this);

    public override DictionaryValue ToDictionaryValue() => new DictionaryValue(this);

    public override bool ToBool() => _value;

    public override double ToDouble() => _value ? 1.0 : 0.0;

    public override string ToString() => _value ? "true" : "false";
}