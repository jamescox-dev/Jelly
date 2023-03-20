namespace Jelly.Ast;

using System;
using Jelly.Values;

public static class Node
{   
    public static DictionaryValue Literal(Value value) =>
        new DictionaryValue(Keywords.Type, Keywords.Literal, Keywords.Value, value);

    public static DictionaryValue Literal(double value) =>
        new DictionaryValue(Keywords.Type, Keywords.Literal, Keywords.Value, value.ToValue());

    public static DictionaryValue Literal(string value) =>
        new DictionaryValue(Keywords.Type, Keywords.Literal, Keywords.Value, value.ToValue());

    public static DictionaryValue Variable(string name) =>
        new DictionaryValue(Keywords.Type, Keywords.Variable, Keywords.Name, name.ToValue());

    public static DictionaryValue Command(DictionaryValue name, ListValue args) =>
        new DictionaryValue(Keywords.Type, Keywords.Command, Keywords.Name, name, Keywords.Args, args);

    public static DictionaryValue Script(params DictionaryValue[] commands) =>
        new DictionaryValue(Keywords.Type, Keywords.Script, Keywords.Commands, commands.ToValue());

    public static DictionaryValue Composite(params DictionaryValue[] parts) =>
        new DictionaryValue(Keywords.Type, Keywords.Composite, Keywords.Parts, parts.ToValue());

    public static DictionaryValue Assignment(string name, DictionaryValue value) =>
        new DictionaryValue(Keywords.Type, Keywords.Assignment, Keywords.Name, name.ToValue(), Keywords.Value, value);

    public static DictionaryValue Expression(params DictionaryValue[] subexpressions) =>
        new DictionaryValue(Keywords.Type, Keywords.Expression, Keywords.Subexpresions, new ListValue(subexpressions));

    public static DictionaryValue BinOp(string op, DictionaryValue a, DictionaryValue b) =>
        new DictionaryValue(Keywords.Type, Keywords.BinOp, Keywords.Op, op.ToValue(), Keywords.A, a, Keywords.B, b);

    public static DictionaryValue UniOp(string op, DictionaryValue a) =>
        new DictionaryValue(Keywords.Type, Keywords.UniOp, Keywords.Op, op.ToValue(), Keywords.A, a);

    public static DictionaryValue DefineVariable(string name, DictionaryValue value) =>
        new DictionaryValue(Keywords.Type, Keywords.DefineVariable, Keywords.Name, name.ToValue(), Keywords.Value, value);

    public static bool IsLiteral(DictionaryValue node) => IsType(node, Keywords.Literal);

    public static bool IsVariable(DictionaryValue node) => IsType(node, Keywords.Variable);

    public static bool IsCommand(DictionaryValue node) => IsType(node, Keywords.Command);

    public static bool IsScript(DictionaryValue node) => IsType(node, Keywords.Script);

    public static bool IsComposite(DictionaryValue node) => IsType(node, Keywords.Composite);

    public static bool IsAssignment(DictionaryValue node) => IsType(node, Keywords.Assignment);

    public static bool IsExprssion(DictionaryValue node) => IsType(node, Keywords.Expression);

    static bool IsType(DictionaryValue node, StringValue type) =>
        node.TryGetValue(Keywords.Type, out var typeValue) && typeValue == type;

    public static Value GetLiteralValue(DictionaryValue literal)
    {
        return literal[Keywords.Value];
    }
}