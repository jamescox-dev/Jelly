namespace Jelly.Values;

using System.Collections;
using System.Collections.Immutable;
using System.Text;

public class DictValue : Value
{
    public static readonly DictValue EmptyDictionary = new();

    readonly ImmutableSortedDictionary<Value, Value> _items;

    public DictValue()
    {
        _items = ImmutableSortedDictionary<Value, Value>.Empty;
    }

    public DictValue(params Value[] list) : this((IEnumerable<Value>)list)
    {
    }

    public DictValue(ImmutableSortedDictionary<Value, Value> dict)
    {
        _items = dict;
    }

    public DictValue(IEnumerable<Value> list)
    {
        var items = ImmutableSortedDictionary.CreateBuilder<Value, Value>();

        Value? key = null;
        foreach (var item in list)
        {
            if (key is null)
            {
                key = item;
            }
            else
            {
                items.Add(key, item);
                key = null;
            }
        }

        if (key is not null)
        {
            items.Add(key, Value.Empty);
        }

        _items = items.ToImmutable();
    }

    public DictValue(IEnumerable<KeyValuePair<Value, Value>> items)
    {
        _items = ImmutableSortedDictionary.CreateRange(items);
    }

    public override ListValue ToListValue()
    {
        var items = ImmutableList.CreateBuilder<Value>();

        foreach (var kvPair in _items)
        {
            items.Add(kvPair.Key);
            items.Add(kvPair.Value);
        }

        return new ListValue(items);
    }

    public override DictValue ToDictValue() => this;

    public Value this[Value key] {
        get
        {
            if (TryGetValue(key, out var value))
            {
                return value;
            }
            throw Error.Key("key does not exist in dictionary.");
        }
    }

    public DictValue SetItem(Value key, Value value) => new(_items.SetItem(key, value));

    public bool ContainsKey(Value key) => _items.ContainsKey(key);

    public bool TryGetValue(Value key, out Value value)
    {
        if (_items.TryGetValue(key, out var itemValue))
        {
            value = itemValue;
            return true;
        }
        value = Value.Empty;
        return false;
    }

    public override string ToString()
    {
        var str = new StringBuilder();

        var first = true;
        foreach (var (name, value) in _items)
        {
            if (!first)
            {
                str.Append("\n");
            }
            else
            {
                first = false;
            }
            str.Append(name.Escape());
            str.Append(" ");
            str.Append(value.Escape());
        }

        return str.ToString();
    }

    public override bool ToBool() => true;

    public override double ToDouble() => double.NaN;

    public IEnumerable<KeyValuePair<Value, Value>> ToEnumerable()
    {
        return _items;
    }
}