namespace Jelly.Values;

using System.Collections;
using System.Collections.Immutable;
using System.Text;

public class DictionaryValue : Value
{
    public static readonly DictionaryValue EmptyDictionary = new();

    readonly ImmutableSortedDictionary<Value, Value> _items;

    public DictionaryValue()
    {
        _items = ImmutableSortedDictionary<Value, Value>.Empty;
    }

    public DictionaryValue(params Value[] list) : this((IEnumerable<Value>)list)
    {
    }

    public DictionaryValue(ImmutableSortedDictionary<Value, Value> dict)
    {
        _items = dict;
    }

    public DictionaryValue(IEnumerable<Value> list)
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

    public DictionaryValue(IEnumerable<KeyValuePair<Value, Value>> items)
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

    public override DictionaryValue ToDictionaryValue() => this;

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

    public DictionaryValue SetItem(Value key, Value value) => new DictionaryValue(_items.SetItem(key, value));

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