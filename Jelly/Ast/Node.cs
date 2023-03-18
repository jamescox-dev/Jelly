namespace Jelly.Ast;

using System;
using Jelly.Values;

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
    static readonly StringValue ExpressionKeyword = new StringValue("expression");
    static readonly StringValue RootKeyword = new StringValue("root");
    static readonly StringValue BinOpKeyword = new StringValue("binop");
    static readonly StringValue OpKeyword = new StringValue("op");
    static readonly StringValue AKeyword = new StringValue("a");
    static readonly StringValue BKeyword = new StringValue("b");
    static readonly StringValue UniOpKeyword = new StringValue("uniop");
    
    public static DictionaryValue Literal(Value value) =>
        new DictionaryValue(TypeKeyword, LiteralKeyword, ValueKeyword, value);

    public static DictionaryValue Literal(double value) =>
        new DictionaryValue(TypeKeyword, LiteralKeyword, ValueKeyword, value.ToValue());

    public static DictionaryValue Literal(string value) =>
        new DictionaryValue(TypeKeyword, LiteralKeyword, ValueKeyword, value.ToValue());

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

    public static DictionaryValue Expression(DictionaryValue root) =>
        new DictionaryValue(TypeKeyword, ExpressionKeyword, RootKeyword, root);

    public static DictionaryValue BinOp(string op, DictionaryValue a, DictionaryValue b) =>
        new DictionaryValue(TypeKeyword, BinOpKeyword, OpKeyword, op.ToValue(), AKeyword, a, BKeyword, b);

    public static DictionaryValue UniOp(string op, DictionaryValue a) =>
        new DictionaryValue(TypeKeyword, UniOpKeyword, OpKeyword, op.ToValue(), AKeyword, a);

    public static bool IsLiteral(DictionaryValue node) => IsType(node, LiteralKeyword);

    public static bool IsVariable(DictionaryValue node) => IsType(node, VariableKeyword);

    public static bool IsCommand(DictionaryValue node) => IsType(node, CommandKeyword);

    public static bool IsScript(DictionaryValue node) => IsType(node, ScriptKeyword);

    public static bool IsComposite(DictionaryValue node) => IsType(node, CompositeKeyword);

    public static bool IsAssignment(DictionaryValue node) => IsType(node, AssignmentKeyword);

    public static bool IsExprssion(DictionaryValue node) => IsType(node, ExpressionKeyword);

    static bool IsType(DictionaryValue node, StringValue type) =>
        node.TryGetValue(TypeKeyword, out var typeValue) && typeValue == type;

    public static Value GetLiteralValue(DictionaryValue literal)
    {
        return literal[ValueKeyword];
    }
}