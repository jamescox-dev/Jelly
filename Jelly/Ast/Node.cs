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

    public static DictionaryValue Literal(bool value) =>
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

    public static DictionaryValue DefineCommand(DictionaryValue name, DictionaryValue body, ListValue argNames, ListValue argDefaults, DictionaryValue? restArgName = null) =>
        restArgName is not null
            ? new DictionaryValue(
                Keywords.Type, Keywords.DefineCommand,
                Keywords.Name, name,
                Keywords.Body, body,
                Keywords.ArgNames, argNames,
                Keywords.ArgDefaults, argDefaults,
                Keywords.RestArgName, restArgName)
            : new DictionaryValue(
                Keywords.Type, Keywords.DefineCommand,
                Keywords.Name, name,
                Keywords.Body, body,
                Keywords.ArgNames, argNames,
                Keywords.ArgDefaults, argDefaults);

    public static DictionaryValue If(DictionaryValue condition, DictionaryValue thenBody, DictionaryValue elseBody) =>
        new DictionaryValue(Keywords.Type, Keywords.If, Keywords.Condition, condition, Keywords.Then, thenBody, Keywords.Else, elseBody);

    public static DictionaryValue If(DictionaryValue condition, DictionaryValue thenBody) =>
        new DictionaryValue(Keywords.Type, Keywords.If, Keywords.Condition, condition, Keywords.Then, thenBody);

    public static DictionaryValue While(DictionaryValue condition, DictionaryValue body) =>
        new DictionaryValue(Keywords.Type, Keywords.While, Keywords.Condition, condition, Keywords.Body, body);

    public static DictionaryValue Scope(DictionaryValue body) =>
        new DictionaryValue(Keywords.Type, Keywords.Scope, Keywords.Body, body);

    public static DictionaryValue Raise(DictionaryValue type, DictionaryValue message, DictionaryValue value) =>
        new DictionaryValue(Keywords.Type, Keywords.Raise, Keywords.ErrorType, type, Keywords.Message, message, Keywords.Value, value);

    public static DictionaryValue Try(DictionaryValue body, DictionaryValue? finallyBody, params (DictionaryValue, DictionaryValue)[] errorHandlers) =>
        finallyBody is null 
            ? new DictionaryValue(
                Keywords.Type, Keywords.Try, 
                Keywords.Body, body,
                Keywords.ErrorHandlers, errorHandlers.Select((errorHandler) =>
                    new ListValue(errorHandler.Item1, errorHandler.Item2)).ToValue())
            : new DictionaryValue(
                Keywords.Type, Keywords.Try, 
                Keywords.Body, body,
                Keywords.Finally, finallyBody, 
                Keywords.ErrorHandlers, errorHandlers.Select((errorHandler) =>
                    new ListValue(errorHandler.Item1, errorHandler.Item2)).ToValue());

    public static DictionaryValue ForRange(DictionaryValue iteratorName, DictionaryValue start, DictionaryValue end, DictionaryValue step, DictionaryValue body) =>
        new DictionaryValue(
            Keywords.Type, Keywords.ForRange,
            Keywords.It, iteratorName,
            Keywords.Start, start,
            Keywords.End, end,
            Keywords.Step, step,
            Keywords.Body, body
        );

    public static DictionaryValue ForList(DictionaryValue indexIteratorName, DictionaryValue valueIteratorName, DictionaryValue list, DictionaryValue body) =>
        new DictionaryValue(
            Keywords.Type, Keywords.ForList,
            Keywords.ItIndex, indexIteratorName,
            Keywords.ItValue, valueIteratorName,
            Keywords.List, list,
            Keywords.Body, body
        );

    public static DictionaryValue ForList(DictionaryValue valueIteratorName, DictionaryValue list, DictionaryValue body) =>
        new DictionaryValue(
            Keywords.Type, Keywords.ForList,
            Keywords.ItValue, valueIteratorName,
            Keywords.List, list,
            Keywords.Body, body
        );

    public static DictionaryValue ForDict(DictionaryValue keyIteratorName, DictionaryValue valueIteratorName, DictionaryValue dict, DictionaryValue body) =>
        new DictionaryValue(
            Keywords.Type, Keywords.ForDict,
            Keywords.ItKey, keyIteratorName,
            Keywords.ItValue, valueIteratorName,
            Keywords.Dict, dict,
            Keywords.Body, body
        );

    public static DictionaryValue ForDict(DictionaryValue valueIteratorName, DictionaryValue dict, DictionaryValue body) =>
        new DictionaryValue(
            Keywords.Type, Keywords.ForDict,
            Keywords.ItValue, valueIteratorName,
            Keywords.Dict, dict,
            Keywords.Body, body
        );

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

    internal static bool IsVariable(Value n)
    {
        throw new NotImplementedException();
    }
}