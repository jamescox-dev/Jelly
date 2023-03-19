namespace Jelly.Ast;

public enum Operator
{
    None,

    Or,
    OrElse,
    And,
    AndThen,
    Not,

    LessThan,
    LessThanOrEqual,
    Equal,
    GreaterThanOrEqual,
    GreaterThan,
    NotEqual,

    BitwiseOr,
    BitwiseXor,
    BitwiseAnd,

    BitshiftLeft,
    BitshiftRight,

    Add,
    Subtract,
    Concatinate,

    Multiply,
    Divide,
    FloorDivide,
    Modulo,
    FloorModulo,

    Positive,
    Negative,
    BitwiseNot,

    Exponent,

    SubexpressionSeparator,
}

public static class OperatorExtentions
{
    public static string GetName(this Operator op) => op switch {
        Operator.Or => "or",
        Operator.OrElse => "orelse",
        Operator.And => "and",
        Operator.AndThen => "andthen",
        Operator.Not => "not",
        Operator.LessThan => "lt",
        Operator.LessThanOrEqual => "lte",
        Operator.Equal => "eq",
        Operator.GreaterThanOrEqual => "gte",
        Operator.GreaterThan => "gt",
        Operator.NotEqual => "ne",
        Operator.BitwiseOr => "bitor",
        Operator.BitwiseXor => "bitxor",
        Operator.BitwiseAnd => "bitand",
        Operator.BitshiftLeft => "lshift",
        Operator.BitshiftRight => "rshift",
        Operator.Add => "add",
        Operator.Subtract => "sub",
        Operator.Concatinate => "cat",
        Operator.Multiply => "mul",
        Operator.Divide => "div",
        Operator.FloorDivide => "floordiv",
        Operator.Modulo => "mod",
        Operator.FloorModulo => "floormod",
        Operator.Positive => "pos",
        Operator.Negative => "neg",
        Operator.BitwiseNot => "bitnot",
        Operator.Exponent => "exp",
        Operator.SubexpressionSeparator => "sep",
        _ => "",
    };

    public static int GetPrecedence(this Operator op) => op switch {
        Operator.None => 0,
        Operator.Or => 1,
        Operator.OrElse => 1,
        Operator.And => 2,
        Operator.AndThen => 2,
        Operator.Not => 3,
        Operator.LessThan => 4,
        Operator.LessThanOrEqual => 4,
        Operator.Equal => 4,
        Operator.GreaterThanOrEqual => 4,
        Operator.GreaterThan => 4,
        Operator.NotEqual => 4,
        Operator.BitwiseOr => 5,
        Operator.BitwiseXor => 6,
        Operator.BitwiseAnd => 7,
        Operator.BitshiftLeft => 8,
        Operator.BitshiftRight => 8,
        Operator.Add => 9,
        Operator.Subtract => 9,
        Operator.Concatinate => 9,
        Operator.Multiply => 10,
        Operator.Divide => 10,
        Operator.FloorDivide => 10,
        Operator.Modulo => 10,
        Operator.FloorModulo => 10,
        Operator.Positive => 11,
        Operator.Negative => 11,
        Operator.BitwiseNot => 11,
        Operator.Exponent => 12,
        Operator.SubexpressionSeparator => int.MaxValue,
        _ => 0,
    };

    public static bool IsBinaryOperator(this Operator op)
    {
        return op switch {
            Operator.BitwiseNot or Operator.Not or Operator.Positive or Operator.Negative => false,
            _ => true
        };
    }
}