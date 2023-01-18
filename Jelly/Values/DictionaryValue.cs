namespace Jelly.Values;

using System.Collections.Immutable;
using System.Text;

public class DictionaryValue : Value
{
    ImmutableSortedDictionary<Value, Value> _items;
    
    public DictionaryValue()
    {
        _items = ImmutableSortedDictionary<Value, Value>.Empty;
    }

    public DictionaryValue(params Value[] list) : this((IEnumerable<Value>)list)
    {   
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

        _items = items.ToImmutable();
    }

    public DictionaryValue(IEnumerable<KeyValuePair<Value, Value>> items)
    {
        _items = ImmutableSortedDictionary.CreateRange(items);
    }

    public override DictionaryValue AsDictionary() => this;

    public Value this[Value key] {
        get => _items[key];
    }

    public override string ToString()
    {
        var str = new StringBuilder();

        var first = true;
        foreach (var (name, value) in _items)
        {
            if (!first)
            {
                str.Append(" ");
            }
            else
            {
                first = false;
            }
            str.Append(name);
            str.Append(" ");
            str.Append(value);
        }

        return str.ToString();
    }
}