namespace Jelly.Values;

using System.Collections.Immutable;
using System.Text;

public class ListValue : Value
{
    ImmutableList<Value> _items;

    public ListValue()
    {
        _items = ImmutableList<Value>.Empty;
    }

    public ListValue(IEnumerable<Value> values)
    {
        _items = ImmutableList.CreateRange(values);
    }

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
}