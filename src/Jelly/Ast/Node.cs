namespace Jelly.Ast;

using System;


public static class Node
{
    public static DictionaryValue Literal(Value value) =>
        new DictionaryValue(Keywords.Type, Keywords.Literal, Keywords.Value, value);

    public static DictionaryValue Literal(Value value, int start, int end) =>
        new DictionaryValue(Keywords.Type, Keywords.Literal, Keywords.Value, value, Keywords.Position, ParsePosition(start, end));

    public static DictionaryValue Literal(double value) => Literal(value.ToValue());

    public static DictionaryValue Literal(double value, int start, int end) => Literal(value.ToValue(), start, end);

    public static DictionaryValue Literal(string value) => Literal(value.ToValue());

    public static DictionaryValue Literal(string value, int start, int end) => Literal(value.ToValue(), start, end);

    public static DictionaryValue Literal(bool value) => Literal(value.ToValue());

    public static DictionaryValue Literal(bool value, int start, int end) => Literal(value.ToValue(), start, end);

    public static DictionaryValue Variable(string name) =>
        new DictionaryValue(Keywords.Type, Keywords.Variable, Keywords.Name, name.ToValue());

    public static DictionaryValue Variable(string name, int start, int end) =>
        new DictionaryValue(Keywords.Type, Keywords.Variable, Keywords.Name, name.ToValue(), Keywords.Position, ParsePosition(start, end));

    public static DictionaryValue Command(DictionaryValue name, ListValue args) =>
        new DictionaryValue(Keywords.Type, Keywords.Command, Keywords.Name, name, Keywords.Args, args);

    public static DictionaryValue Command(DictionaryValue name, ListValue args, int start, int end) =>
        new DictionaryValue(Keywords.Type, Keywords.Command, Keywords.Name, name, Keywords.Args, args, Keywords.Position, ParsePosition(start, end));

    public static DictionaryValue Script(params DictionaryValue[] commands) =>
        new DictionaryValue(Keywords.Type, Keywords.Script, Keywords.Commands, commands.ToValue());

    public static DictionaryValue Script(int start, int end, params DictionaryValue[] commands) =>
        new DictionaryValue(Keywords.Type, Keywords.Script, Keywords.Commands, commands.ToValue(), Keywords.Position, ParsePosition(start, end));

    public static DictionaryValue Composite(params DictionaryValue[] parts) =>
        new DictionaryValue(Keywords.Type, Keywords.Composite, Keywords.Parts, parts.ToValue());

    public static DictionaryValue Composite(int start, int end, params DictionaryValue[] parts) =>
        new DictionaryValue(Keywords.Type, Keywords.Composite, Keywords.Parts, parts.ToValue(), Keywords.Position, ParsePosition(start, end));

    public static DictionaryValue Assignment(string name, DictionaryValue value) =>
        new DictionaryValue(Keywords.Type, Keywords.Assignment, Keywords.Name, name.ToValue(), Keywords.Value, value);

    public static DictionaryValue Assignment(string name, DictionaryValue value, int start, int end) =>
        new DictionaryValue(Keywords.Type, Keywords.Assignment, Keywords.Name, name.ToValue(), Keywords.Value, value, Keywords.Position, ParsePosition(start, end));

    public static DictionaryValue Expression(params DictionaryValue[] subExpressions) =>
        new DictionaryValue(Keywords.Type, Keywords.Expression, Keywords.SubExpressions, new ListValue(subExpressions));

    public static DictionaryValue Expression(int start, int end, params DictionaryValue[] subExpressions) =>
        new DictionaryValue(Keywords.Type, Keywords.Expression, Keywords.SubExpressions, new ListValue(subExpressions), Keywords.Position, ParsePosition(start, end));

    public static DictionaryValue BinOp(string op, DictionaryValue a, DictionaryValue b) =>
        new DictionaryValue(Keywords.Type, Keywords.BinOp, Keywords.Op, op.ToValue(), Keywords.A, a, Keywords.B, b);

    public static DictionaryValue BinOp(int start, int end, string op, DictionaryValue a, DictionaryValue b) =>
        new DictionaryValue(Keywords.Type, Keywords.BinOp, Keywords.Op, op.ToValue(), Keywords.A, a, Keywords.B, b, Keywords.Position, ParsePosition(start, end));

    public static DictionaryValue UniOp(string op, DictionaryValue a) =>
        new DictionaryValue(Keywords.Type, Keywords.UniOp, Keywords.Op, op.ToValue(), Keywords.A, a);

    public static DictionaryValue UniOp(int start, int end, string op, DictionaryValue a) =>
        new DictionaryValue(Keywords.Type, Keywords.UniOp, Keywords.Op, op.ToValue(), Keywords.A, a, Keywords.Position, ParsePosition(start, end));

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

    public static DictionaryValue ForRange(DictionaryValue iteratorName, DictionaryValue start, DictionaryValue end, DictionaryValue body) =>
        new DictionaryValue(
            Keywords.Type, Keywords.ForRange,
            Keywords.It, iteratorName,
            Keywords.Start, start,
            Keywords.End, end,
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

    public static DictionaryValue ForDict(DictionaryValue keyIteratorName, DictionaryValue dict, DictionaryValue body) =>
        new DictionaryValue(
            Keywords.Type, Keywords.ForDict,
            Keywords.ItKey, keyIteratorName,
            Keywords.Dict, dict,
            Keywords.Body, body
        );

    public static DictionaryValue Reposition(DictionaryValue node, int start, int end) => node.SetItem(Keywords.Position, ParsePosition(start, end));

    public static DictionaryValue Reposition(DictionaryValue node, DictionaryValue other)
    {
        var otherPos = GetPosition(other);
        if (otherPos is not null)
        {
            return node.SetItem(Keywords.Position, otherPos);
        }
        return node;
    }

    public static DictionaryValue Reposition(DictionaryValue node, DictionaryValue start, DictionaryValue end)
    {
        var startPos = GetPosition(start);
        var endPos = GetPosition(end);
        if (startPos is not null && endPos is not null)
        {
            return node.SetItem(Keywords.Position, ParsePosition((int)startPos[Keywords.Start].ToDouble(), (int)endPos[Keywords.End].ToDouble()));
        }
        return node;
    }

    public static DictionaryValue? GetPosition(DictionaryValue node)
    {
        if (node.TryGetValue(Keywords.Position, out var positionValue))
        {
            var positionDictionary = positionValue.ToNode();
            if (positionDictionary.ContainsKey(Keywords.Start) && positionDictionary.ContainsKey(Keywords.End))
            {
                return positionDictionary;
            }
        }
        return null;
    }

    public static bool IsLiteral(DictionaryValue node) => IsType(node, Keywords.Literal);

    public static bool IsVariable(DictionaryValue node) => IsType(node, Keywords.Variable);

    public static bool IsCommand(DictionaryValue node) => IsType(node, Keywords.Command);

    public static bool IsScript(DictionaryValue node) => IsType(node, Keywords.Script);

    public static bool IsComposite(DictionaryValue node) => IsType(node, Keywords.Composite);

    public static bool IsAssignment(DictionaryValue node) => IsType(node, Keywords.Assignment);

    public static bool IsExpression(DictionaryValue node) => IsType(node, Keywords.Expression);

    public static bool IsKeyword(DictionaryValue node, string keyword) =>
        IsLiteral(node) && GetLiteralValue(node).ToString().Equals(keyword, StringComparison.InvariantCultureIgnoreCase);

    static bool IsType(DictionaryValue node, StringValue type) =>
        node.TryGetValue(Keywords.Type, out var typeValue) && typeValue == type;

    public static Value GetLiteralValue(DictionaryValue literal)
    {
        return literal[Keywords.Value];
    }

    public static DictionaryValue ToLiteralIfVariable(DictionaryValue node)
    {
        if (Node.IsVariable(node))
        {
            return Node.Literal(node.GetString(Keywords.Name));
        }
        return node;
    }

    static DictionaryValue ParsePosition(int start, int end) =>
        new DictionaryValue(Keywords.Start, start.ToValue(), Keywords.End, end.ToValue());
}