using Jelly.Values;

namespace Jelly.Parser;

public class NodeBuilder
{
    public static readonly NodeBuilder Shared = new NodeBuilder();

    static readonly StringValue TypeKeyword = new StringValue("type");
    static readonly StringValue LiteralKeyword = new StringValue("literal");
    static readonly StringValue VariableKeyword = new StringValue("variable");
    static readonly StringValue ValueKeyword = new StringValue("value");
    static readonly StringValue CommandKeyword = new StringValue("command");
    static readonly StringValue NameKeyword = new StringValue("name");
    static readonly StringValue ArgsKeyword = new StringValue("args");
    static readonly StringValue ScriptKeyword = new StringValue("script");
    static readonly StringValue CommandsKeyword = new StringValue("commands");
    static readonly StringValue CompositeKeyword = new StringValue("composite");
    static readonly StringValue PartsKeyword = new StringValue("parts");
    
    public DictionaryValue Literal(Value value)
    {
        return new DictionaryValue(TypeKeyword, LiteralKeyword, ValueKeyword, value);
    }

    public DictionaryValue Variable(string name)
    {
        return new DictionaryValue(TypeKeyword, VariableKeyword, NameKeyword, name.ToValue());
    }

    public DictionaryValue Command(DictionaryValue name, ListValue args)
    {
        return new DictionaryValue(TypeKeyword, CommandKeyword, NameKeyword, name, ArgsKeyword, args);
    }

    public DictionaryValue Script(params DictionaryValue[] commands)
    {
        return new DictionaryValue(TypeKeyword, ScriptKeyword, CommandsKeyword, commands.ToValue());
    }

    public DictionaryValue Composite(params DictionaryValue[] parts)
    {
        return new DictionaryValue(TypeKeyword, CompositeKeyword, PartsKeyword, parts.ToValue());
    }
}