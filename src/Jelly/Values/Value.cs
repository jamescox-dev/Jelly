namespace Jelly.Values;

public abstract class Value : IEquatable<Value>, IComparable<Value>
{
    public static readonly Value Empty = new StrValue(string.Empty);

    public string Escape() => ValueSerializer.Escape(ToString());

    public abstract ListValue ToListValue();

    public abstract DictValue ToDictionaryValue();

    public abstract override string ToString();

    public abstract bool ToBool();

    public abstract double ToDouble();

    public int CompareTo(Value? other)
    {
        if (other is Value value)
        {
            return StringComparer.InvariantCulture.Compare(ToString(), value.ToString());
        }
        return StringComparer.InvariantCulture.Compare(ToString(), "");
    }

    public bool Equals(Value? other) =>
        CompareTo(other) == 0;

    public override bool Equals(object? obj)
    {
        if (obj is Value value)
        {
            return Equals(value);
        }
        return false;
    }

    public override int GetHashCode() => StringComparer.InvariantCulture.GetHashCode(ToString());
}