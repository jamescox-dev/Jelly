using Jelly.Serializers;

namespace Jelly.Values;

public abstract class Value : IEquatable<Value>, IComparable<Value>
{
  public static readonly Value Empty = new StrValue(string.Empty);

  public string Escape() => ValueSerializer.Escape(ToString());

  public virtual BoolValue ToBoolValue() => ToBool() ? BoolValue.True : BoolValue.False;

  public virtual NumValue ToNumValue() => new(ToDouble());

  public virtual StrValue ToStrValue() => new(ToString());

  public abstract ListValue ToListValue();

  public abstract DictValue ToDictValue();

  public abstract override string ToString();

  public abstract bool ToBool();

  public abstract double ToDouble();

  public int CompareTo(Value? other)
  {
    if (other is Value value)
    {
      return StringComparer.InvariantCultureIgnoreCase.Compare(ToString(), value.ToString());
    }
    return StringComparer.InvariantCultureIgnoreCase.Compare(ToString(), "");
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

  public override int GetHashCode() => StringComparer.InvariantCultureIgnoreCase.GetHashCode(ToString());
}
