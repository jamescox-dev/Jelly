namespace Jelly.Ast;

using System;


public static class Node
{
    public static DictValue Literal(Value value) =>
        new(Keywords.Type, Keywords.Literal, Keywords.Value, value);

    public static DictValue Literal(Value value, int start, int end) =>
        new(Keywords.Type, Keywords.Literal, Keywords.Value, value, Keywords.Position, ParsePosition(start, end));

    public static DictValue Literal(double value) => Literal(value.ToValue());

    public static DictValue Literal(double value, int start, int end) => Literal(value.ToValue(), start, end);

    public static DictValue Literal(string value) => Literal(value.ToValue());

    public static DictValue Literal(string value, int start, int end) => Literal(value.ToValue(), start, end);

    public static DictValue Literal(bool value) => Literal(value.ToValue());

    public static DictValue Literal(bool value, int start, int end) => Literal(value.ToValue(), start, end);

    public static DictValue Variable(string name) =>
        new(Keywords.Type, Keywords.Variable, Keywords.Name, name.ToValue());

    public static DictValue Variable(string name, int start, int end) =>
        new(Keywords.Type, Keywords.Variable, Keywords.Name, name.ToValue(), Keywords.Position, ParsePosition(start, end));

    public static DictValue Variable(int start, int end, string name, params DictValue[] indexers) =>
        new(Keywords.Type, Keywords.Variable, Keywords.Name, name.ToValue(), Keywords.Indexers, new ListValue(indexers), Keywords.Position, ParsePosition(start, end));

    public static DictValue ListIndexer(int start, int end, DictValue expression) =>
        new(Keywords.Type, Keywords.ListIndexer, Keywords.Expression, expression, Keywords.Position, ParsePosition(start, end));

    public static DictValue DictIndexer(int start, int end, DictValue expression) =>
        new(Keywords.Type, Keywords.DictIndexer, Keywords.Expression, expression, Keywords.Position, ParsePosition(start, end));

    public static DictValue Command(DictValue name, ListValue args) =>
        new(Keywords.Type, Keywords.Command, Keywords.Name, name, Keywords.Args, args);

    public static DictValue Command(DictValue name, ListValue args, int start, int end) =>
        new(Keywords.Type, Keywords.Command, Keywords.Name, name, Keywords.Args, args, Keywords.Position, ParsePosition(start, end));

    public static DictValue Script(params DictValue[] commands) =>
        new(Keywords.Type, Keywords.Script, Keywords.Commands, commands.ToValue());

    public static DictValue Script(int start, int end, params DictValue[] commands) =>
        new(Keywords.Type, Keywords.Script, Keywords.Commands, commands.ToValue(), Keywords.Position, ParsePosition(start, end));

    public static DictValue Composite(params DictValue[] parts) =>
        new(Keywords.Type, Keywords.Composite, Keywords.Parts, parts.ToValue());

    public static DictValue Composite(int start, int end, params DictValue[] parts) =>
        new(Keywords.Type, Keywords.Composite, Keywords.Parts, parts.ToValue(), Keywords.Position, ParsePosition(start, end));

    public static DictValue Assignment(string name, DictValue value) =>
        new(Keywords.Type, Keywords.Assignment, Keywords.Name, name.ToValue(), Keywords.Value, value);

    public static DictValue Assignment(string name, DictValue value, int start, int end) =>
        new(Keywords.Type, Keywords.Assignment, Keywords.Name, name.ToValue(), Keywords.Value, value, Keywords.Position, ParsePosition(start, end));

    public static DictValue Assignment(int start, int end, string name, DictValue value, params DictValue[] indexers) =>
        new(Keywords.Type, Keywords.Assignment, Keywords.Name, name.ToValue(), Keywords.Value, value, Keywords.Indexers, new ListValue(indexers), Keywords.Position, ParsePosition(start, end));

    public static DictValue Expression(params DictValue[] subExpressions) =>
        new(Keywords.Type, Keywords.Expression, Keywords.SubExpressions, new ListValue(subExpressions));

    public static DictValue Expression(int start, int end, params DictValue[] subExpressions) =>
        new(Keywords.Type, Keywords.Expression, Keywords.SubExpressions, new ListValue(subExpressions), Keywords.Position, ParsePosition(start, end));

    public static DictValue BinOp(string op, DictValue a, DictValue b) =>
        new(Keywords.Type, Keywords.BinOp, Keywords.Op, op.ToValue(), Keywords.A, a, Keywords.B, b);

    public static DictValue BinOp(int start, int end, string op, DictValue a, DictValue b) =>
        new(Keywords.Type, Keywords.BinOp, Keywords.Op, op.ToValue(), Keywords.A, a, Keywords.B, b, Keywords.Position, ParsePosition(start, end));

    public static DictValue UniOp(string op, DictValue a) =>
        new(Keywords.Type, Keywords.UniOp, Keywords.Op, op.ToValue(), Keywords.A, a);

    public static DictValue UniOp(int start, int end, string op, DictValue a) =>
        new(Keywords.Type, Keywords.UniOp, Keywords.Op, op.ToValue(), Keywords.A, a, Keywords.Position, ParsePosition(start, end));

    public static DictValue DefineVariable(string name, DictValue value) =>
        new(Keywords.Type, Keywords.DefineVariable, Keywords.Name, name.ToValue(), Keywords.Value, value);

    public static DictValue DefineCommand(DictValue name, DictValue body, ListValue argNames, ListValue argDefaults, DictValue? restArgName = null) =>
        restArgName is not null
            ? new(
                Keywords.Type, Keywords.DefineCommand,
                Keywords.Name, name,
                Keywords.Body, body,
                Keywords.ArgNames, argNames,
                Keywords.ArgDefaults, argDefaults,
                Keywords.RestArgName, restArgName)
            : new(
                Keywords.Type, Keywords.DefineCommand,
                Keywords.Name, name,
                Keywords.Body, body,
                Keywords.ArgNames, argNames,
                Keywords.ArgDefaults, argDefaults);

    public static DictValue If(DictValue condition, DictValue thenBody, DictValue elseBody) =>
        new(Keywords.Type, Keywords.If, Keywords.Condition, condition, Keywords.Then, thenBody, Keywords.Else, elseBody);

    public static DictValue If(DictValue condition, DictValue thenBody) =>
        new(Keywords.Type, Keywords.If, Keywords.Condition, condition, Keywords.Then, thenBody);

    public static DictValue While(DictValue condition, DictValue body) =>
        new(Keywords.Type, Keywords.While, Keywords.Condition, condition, Keywords.Body, body);

    public static DictValue Scope(DictValue body) =>
        new(Keywords.Type, Keywords.Scope, Keywords.Body, body);

    public static DictValue Raise(DictValue type, DictValue message, DictValue value) =>
        new(Keywords.Type, Keywords.Raise, Keywords.ErrorType, type, Keywords.Message, message, Keywords.Value, value);

    public static DictValue Try(DictValue body, DictValue? finallyBody, params (DictValue, DictValue)[] errorHandlers) =>
        finallyBody is null
            ? new(
                Keywords.Type, Keywords.Try,
                Keywords.Body, body,
                Keywords.ErrorHandlers, errorHandlers.Select((errorHandler) =>
                    new ListValue(errorHandler.Item1, errorHandler.Item2)).ToValue())
            : new(
                Keywords.Type, Keywords.Try,
                Keywords.Body, body,
                Keywords.Finally, finallyBody,
                Keywords.ErrorHandlers, errorHandlers.Select((errorHandler) =>
                    new ListValue(errorHandler.Item1, errorHandler.Item2)).ToValue());

    public static DictValue ForRange(DictValue iteratorName, DictValue start, DictValue end, DictValue step, DictValue body) =>
        new(
            Keywords.Type, Keywords.ForRange,
            Keywords.It, iteratorName,
            Keywords.Start, start,
            Keywords.End, end,
            Keywords.Step, step,
            Keywords.Body, body
        );

    public static DictValue ForRange(DictValue iteratorName, DictValue start, DictValue end, DictValue body) =>
        new(
            Keywords.Type, Keywords.ForRange,
            Keywords.It, iteratorName,
            Keywords.Start, start,
            Keywords.End, end,
            Keywords.Body, body
        );

    public static DictValue ForList(DictValue indexIteratorName, DictValue valueIteratorName, DictValue list, DictValue body) =>
        new(
            Keywords.Type, Keywords.ForList,
            Keywords.ItIndex, indexIteratorName,
            Keywords.ItValue, valueIteratorName,
            Keywords.List, list,
            Keywords.Body, body
        );

    public static DictValue ForList(DictValue valueIteratorName, DictValue list, DictValue body) =>
        new(
            Keywords.Type, Keywords.ForList,
            Keywords.ItValue, valueIteratorName,
            Keywords.List, list,
            Keywords.Body, body
        );

    public static DictValue ForDict(DictValue keyIteratorName, DictValue valueIteratorName, DictValue dict, DictValue body) =>
        new(
            Keywords.Type, Keywords.ForDict,
            Keywords.ItKey, keyIteratorName,
            Keywords.ItValue, valueIteratorName,
            Keywords.Dict, dict,
            Keywords.Body, body
        );

    public static DictValue ForDict(DictValue keyIteratorName, DictValue dict, DictValue body) =>
        new(
            Keywords.Type, Keywords.ForDict,
            Keywords.ItKey, keyIteratorName,
            Keywords.Dict, dict,
            Keywords.Body, body
        );

    public static DictValue Reposition(DictValue node, int start, int end) => node.SetItem(Keywords.Position, ParsePosition(start, end));

    public static DictValue Reposition(DictValue node, DictValue other)
    {
        var otherPos = GetPosition(other);
        if (otherPos is not null)
        {
            return node.SetItem(Keywords.Position, otherPos);
        }
        return node;
    }

    public static DictValue Reposition(DictValue node, DictValue start, DictValue end)
    {
        var startPos = GetPosition(start);
        var endPos = GetPosition(end);
        if (startPos is not null && endPos is not null)
        {
            return node.SetItem(Keywords.Position, ParsePosition((int)startPos[Keywords.Start].ToDouble(), (int)endPos[Keywords.End].ToDouble()));
        }
        return node;
    }

    public static DictValue? GetPosition(DictValue node)
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

    public static int GetStartPosition(DictValue node)
    {
        var position = GetPosition(node);
        if (position is not null)
        {
            return (int)position[Keywords.Start].ToDouble();
        }
        return -1;
    }

    public static int GetEndPosition(DictValue node)
    {
        var position = GetPosition(node);
        if (position is not null)
        {
            return (int)position[Keywords.End].ToDouble();
        }
        return -1;
    }

    public static bool IsLiteral(DictValue node) => IsType(node, Keywords.Literal);

    public static bool IsVariable(DictValue node) => IsType(node, Keywords.Variable);

    public static bool IsCommand(DictValue node) => IsType(node, Keywords.Command);

    public static bool IsScript(DictValue node) => IsType(node, Keywords.Script);

    public static bool IsComposite(DictValue node) => IsType(node, Keywords.Composite);

    public static bool IsAssignment(DictValue node) => IsType(node, Keywords.Assignment);

    public static bool IsExpression(DictValue node) => IsType(node, Keywords.Expression);

    public static bool IsKeyword(DictValue node, string keyword) =>
        IsLiteral(node) && GetLiteralValue(node).ToString().Equals(keyword, StringComparison.InvariantCultureIgnoreCase);

    static bool IsType(DictValue node, StrValue type) =>
        node.TryGetValue(Keywords.Type, out var typeValue) && typeValue == type;

    public static Value GetLiteralValue(DictValue literal)
    {
        return literal[Keywords.Value];
    }

    public static DictValue ToLiteralIfVariable(DictValue node)
    {
        if (IsVariable(node))
        {
            if (node.ContainsKey(Keywords.Indexers))
            {
                var indexers = node[Keywords.Indexers].ToListValue();
                var error = Error.Arg("variable name must not include indexers.");
                error.StartPosition = (int)GetPosition(indexers[0].ToNode())![Keywords.Start].ToDouble();
                error.EndPosition = (int)GetPosition(indexers[^1].ToNode())![Keywords.End].ToDouble();
                throw error;
            }
            return Reposition(Literal(node.GetString(Keywords.Name)), node);
        }
        return node;
    }

    static DictValue ParsePosition(int start, int end) =>
        new(Keywords.Start, start.ToValue(), Keywords.End, end.ToValue());
}