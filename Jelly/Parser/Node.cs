using Jelly.Values;

namespace Jelly.Parser;

public static class Node
{
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
    static readonly StringValue AssignmentKeyword = new StringValue("assignment");
    
    public static DictionaryValue Literal(Value value) =>
        new DictionaryValue(TypeKeyword, LiteralKeyword, ValueKeyword, value);

    public static DictionaryValue Variable(string name) => 
        new DictionaryValue(TypeKeyword, VariableKeyword, NameKeyword, name.ToValue());
    
    public static DictionaryValue Command(DictionaryValue name, ListValue args) =>
        new DictionaryValue(TypeKeyword, CommandKeyword, NameKeyword, name, ArgsKeyword, args);

    public static DictionaryValue Script(params DictionaryValue[] commands) =>
        new DictionaryValue(TypeKeyword, ScriptKeyword, CommandsKeyword, commands.ToValue());

    public static DictionaryValue Composite(params DictionaryValue[] parts) =>
        new DictionaryValue(TypeKeyword, CompositeKeyword, PartsKeyword, parts.ToValue());

    public static DictionaryValue Assignment(string name, DictionaryValue value) =>
        new DictionaryValue(TypeKeyword, AssignmentKeyword, NameKeyword, name.ToValue(), ValueKeyword, value);
}