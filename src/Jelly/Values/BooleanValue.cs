namespace Jelly.Values;

public class BoolValue : Value
{
    public static readonly BoolValue True = new BoolValue(true);
    public static readonly BoolValue False = new BoolValue(false);

    bool _value;

    private BoolValue(bool value)
    {
        _value = value;
    }

    public override ListValue ToListValue() => new ListValue(this);

    public override DictionaryValue ToDictionaryValue() => new DictionaryValue(this);

    public override bool ToBool() => _value;

    public override double ToDouble() => _value ? 1.0 : 0.0;

    public override string ToString() => _value ? "true" : "false";
}