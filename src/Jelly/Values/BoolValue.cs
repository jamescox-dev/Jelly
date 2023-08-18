namespace Jelly.Values;

public class BoolValue : Value
{
    public static readonly BoolValue True = new(true);
    public static readonly BoolValue False = new(false);

    readonly bool _value;

    BoolValue(bool value)
    {
        _value = value;
    }

    public override BoolValue ToBoolValue() => this;

    public override ListValue ToListValue() => new(this);

    public override DictValue ToDictValue() => new(this);

    public override bool ToBool() => _value;

    public override double ToDouble() => _value ? 1.0 : 0.0;

    public override string ToString() => _value ? "true" : "false";
}