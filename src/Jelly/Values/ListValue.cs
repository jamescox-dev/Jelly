namespace Jelly.Values;

using System.Collections;
using System.Collections.Immutable;
using System.Text;

public class ListValue : Value, IEnumerable<Value>
{
    public static readonly ListValue EmptyList = new();

    readonly ImmutableList<Value> _items;

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
        get
        {
            if (index >= 0 && index < Count)
            {
                return _items[index];
            }
            throw new IndexError("index out of bounds.");
        }
    }

    public ListValue SetItem(int index, Value value)
    {
        if (index >= 0 && index < Count)
        {
            return new ListValue(_items.SetItem(index, value));
        }
        throw new IndexError("index out of bounds.");
    }

    public int Count => _items.Count;

    public ListValue Add(Value value) => new(_items.Add(value));

    public ListValue AddRange(ListValue list) => new(_items.AddRange(list));

    public ListValue InsertRange(int index, ListValue list) => new(_items.InsertRange(index, list));

    public ListValue RemoveAtRange(params int[] indices) => RemoveAtRange((IEnumerable<int>)indices);

    public ListValue RemoveAtRange(IEnumerable<int> indices)
    {
        try
        {
            var newItems = _items;
            foreach (var index in indices.OrderByDescending(i => i))
            {
                newItems = newItems.RemoveAt(index);
            }
            return new(newItems);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new IndexError("index out of bounds.");
        }
    }

    public override ListValue ToListValue() => this;

    public override DictValue ToDictValue() => new((IEnumerable<Value>)this);

    public IEnumerator<Value> GetEnumerator() => _items.GetEnumerator();

    public override string ToString()
    {
        var str = new StringBuilder();

        var first = true;
        foreach (var value in _items)
        {
            if (!first)
            {
                str.Append('\n');
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