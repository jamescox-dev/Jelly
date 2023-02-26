namespace Jelly.Values;

using Jelly.Serializers;

public abstract class Value : IEquatable<Value>, IComparable<Value>
{
    public static readonly Value Empty = new StringValue(string.Empty);

    public string Escape() => ValueSerializer.Escape(ToString());

    public virtual ListValue ToListValue()
    {
        throw new NotImplementedException();
    }

    public virtual DictionaryValue ToDictionaryValue()
    {
        throw new NotImplementedException();
    }

    public abstract override string ToString();

    public virtual double ToDouble()
    {
        return NumberValue.ParseNumber(ToString());
    }

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