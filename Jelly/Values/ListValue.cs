namespace Jelly.Values;

using System.Collections;
using System.Collections.Immutable;
using System.Text;

public class ListValue : Value, IEnumerable<Value>
{
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

    public Value this[int index]
    {
        get => _items[index];
    }

    public int Count => _items.Count;

    public override ListValue ToListValue() => this;

    public IEnumerator<Value> GetEnumerator() => _items.GetEnumerator();

    public override string ToString()
    {
        var str = new StringBuilder();

        var first = true;
        foreach (var value in _items)
        {
            if (!first)
            {
                str.Append(" ");
            }
            else
            {
                first = false;
            }
            str.Append(value);
        }

        return str.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Value>)this).GetEnumerator();
}