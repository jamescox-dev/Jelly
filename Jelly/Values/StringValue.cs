namespace Jelly.Values;

public class StringValue : Value
{
    readonly string _value;

    public StringValue(string value)
    {
        _value = value;
    }

    public override string ToString() => _value;
}