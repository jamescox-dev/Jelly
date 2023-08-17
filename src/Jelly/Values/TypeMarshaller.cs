namespace Jelly.Values;

using System.Collections;

public class TypeMarshaller : ITypeMarshaller
{
    public static readonly ITypeMarshaller Shared = new TypeMarshaller();

    public Value Marshal(object? clrValue)
    {
        if (clrValue is Value value)
        {
            return value;
        }
        if (clrValue is null)
        {
            return Value.Empty;
        }
        if (clrValue is bool boolValue)
        {
            return boolValue.ToValue();
        }
        if (clrValue is int intValue)
        {
            return intValue.ToValue();
        }
        if (clrValue is double doubleValue)
        {
            return doubleValue.ToValue();
        }
        if (clrValue is string stringValue)
        {
            return stringValue.ToValue();
        }
        if (clrValue is IDictionary dictionaryValue)
        {
            return new DictValue(dictionaryValue.Keys.Cast<object>().Select(k =>
                new KeyValuePair<Value, Value>(Marshal(k), Marshal(dictionaryValue[k]))));
        }
        if (clrValue is IEnumerable enumerableValue)
        {
            return new ListValue(enumerableValue.Cast<object>().Select(Marshal));
        }
        throw Error.BuildType("Unsupported CLR type.");
    }

    public object Marshal(Value jellyValue, Type clrType)
    {
        if (clrType == typeof(bool))
        {
            return jellyValue.ToBool();
        }
        if (clrType == typeof(int))
        {
            var doubleValue = jellyValue.ToDouble();
            if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
            {
                throw Error.BuildType("Invalid integer.");
            }
            return (int)(uint)doubleValue;
        }
        if (clrType == typeof(double))
        {
            return jellyValue.ToDouble();
        }
        if (clrType == typeof(string))
        {
            return jellyValue.ToString();
        }
        if (clrType == typeof(Value))
        {
            return jellyValue;
        }
        if (clrType == typeof(BoolValue))
        {
            return jellyValue.ToBool() ? BoolValue.True : BoolValue.False;
        }
        if (clrType == typeof(NumValue))
        {
            return new NumValue(jellyValue.ToDouble());
        }
        if (clrType == typeof(StrValue))
        {
            return new StrValue(jellyValue.ToString());
        }
        if (clrType == typeof(ListValue))
        {
            return jellyValue.ToListValue();
        }
        if (clrType == typeof(DictValue))
        {
            return jellyValue.ToDictValue();
        }
        if (TryMarshalToEnumerable(jellyValue, clrType, (v) => v.ToBool(), out var enumerableOfBoolValue))
        {
            return enumerableOfBoolValue;
        }
        if (TryMarshalToEnumerable(jellyValue, clrType, (v) => (int)Marshal(v, typeof(int)), out var enumerableOfIntValue))
        {
            return enumerableOfIntValue;
        }
        if (TryMarshalToEnumerable(jellyValue, clrType, (v) => v.ToDouble(), out var enumerableOfDoubleValue))
        {
            return enumerableOfDoubleValue;
        }
        if (TryMarshalToEnumerable(jellyValue, clrType, (v) => v.ToString(), out var enumerableOfStringValue))
        {
            return enumerableOfStringValue;
        }
        throw Error.BuildType("Unsupported CLR type.");
    }

    bool TryMarshalToEnumerable<T>(Value jellyValue, Type clrType, Func<Value, T> convertionFunc, out object enumerableValue)
    {
        if (clrType.IsAssignableTo(typeof(IEnumerable<T>)))
        {
            var constructor = clrType.GetConstructor(new[] { typeof(IEnumerable<T>) });
            if (constructor is not null)
            {
                enumerableValue = constructor.Invoke(new object?[] { jellyValue.ToListValue().Select(convertionFunc) });
                return true;
            }
        }
        enumerableValue = null!;
        return false;
    }
}