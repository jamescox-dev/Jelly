namespace Jelly.Values;

using System.Collections;
using System.Collections.Immutable;
using System.Text;

public class ListValue : Value, IEnumerable<Value>
{
    public static readonly ListValue EmptyList = new();

    ImmutableList<Value> _items;

    public ListValue()
    {
        _items = ImmutableList<Value>.Empty;
    }

    public ListValue(params Value[] values) : this((IEnumerable<Value>)values)
    {
    }

    public ListValue(IEnumerable<Value> values)
    {
        _items = ImmutableList.CreateRange(values);
    }

    public ListValue(ImmutableList<Value> values)
    {
        _items = values;
    }

    public Value this[int index]
    {
        get => _items[index];
    }

    public int Count => _items.Count;

    public override ListValue ToListValue() => this;

    public override DictionaryValue ToDictionaryValue() => new DictionaryValue((IEnumerable<Value>)this);

    public IEnumerator<Value> GetEnumerator() => _items.GetEnumerator();

    public override string ToString()
    {
        var str = new StringBuilder();

        var first = true;
        foreach (var value in _items)
        {
            if (!first)
            {
                str.Append("\n");
            }
            else
            {
                first = false;
            }
            str.Append(value.Escape());
        }

        return str.ToString();
    }

    public override bool ToBool() => _items.Count != 1 || (_items.Count == 1 && _items[0].ToBool());

    public override double ToDouble()
    {
        if (_items.Count == 1)
        {
            return _items[0].ToDouble();
        }
        return double.NaN;
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Value>)this).GetEnumerator();
}