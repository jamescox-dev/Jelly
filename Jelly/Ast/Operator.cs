namespace Jelly.Ast;

public enum Operator
{
    None,

    Or,
    OrElse,
    And,
    AndThen,
    Not,

    StrEqual,
    StrNotEqual,
    // TODO:  StrLike (like),

    // TODO:  CaseInsensitiveStrEqual (ieq),
    // TODO:  CaseInsensitiveStrNotEqual (ine),
    // TODO:  CaseInsensitiveStrLike (ilike),

    // TODO:  ListIn (in),

    // TODO:  DictHas (has),

    LessThan,
    LessThanOrEqual,
    Equal,
    GreaterThanOrEqual,
    GreaterThan,
    NotEqual,

    BitwiseOr,
    BitwiseXor,
    BitwiseAnd,

    BitShiftLeft,
    BitShiftRight,

    Add,
    Subtract,
    Concatenate,

    Multiply,
    Divide,
    FloorDivide,
    Modulo,
    FloorModulo,

    Positive,
    Negative,
    BitwiseNot,

    Exponent,

    SubExpressionSeparator,
}

public static class OperatorExtentions
{
    public static string GetName(this Operator op) => op switch {
        Operator.Or => "or",
        Operator.OrElse => "orelse",
        Operator.And => "and",
        Operator.AndThen => "andthen",
        Operator.Not => "not",
        Operator.StrEqual => "streq",
        Operator.StrNotEqual => "strne",
        Operator.LessThan => "lt",
        Operator.LessThanOrEqual => "lte",
        Operator.Equal => "eq",
        Operator.GreaterThanOrEqual => "gte",
        Operator.GreaterThan => "gt",
        Operator.NotEqual => "ne",
        Operator.BitwiseOr => "bitor",
        Operator.BitwiseXor => "bitxor",
        Operator.BitwiseAnd => "bitand",
        Operator.BitShiftLeft => "lshift",
        Operator.BitShiftRight => "rshift",
        Operator.Add => "add",
        Operator.Subtract => "sub",
        Operator.Concatenate => "cat",
        Operator.Multiply => "mul",
        Operator.Divide => "div",
        Operator.FloorDivide => "floordiv",
        Operator.Modulo => "mod",
        Operator.FloorModulo => "floormod",
        Operator.Positive => "pos",
        Operator.Negative => "neg",
        Operator.BitwiseNot => "bitnot",
        Operator.Exponent => "exp",
        Operator.SubExpressionSeparator => "sep",
        _ => "",
    };

    public static int GetPrecedence(this Operator op) => op switch {
        Operator.None => 0,
        Operator.Or => 1,
        Operator.OrElse => 1,
        Operator.And => 2,
        Operator.AndThen => 2,
        Operator.Not => 3,
        Operator.StrEqual => 4,
        Operator.StrNotEqual => 4,
        Operator.LessThan => 4,
        Operator.LessThanOrEqual => 4,
        Operator.Equal => 4,
        Operator.GreaterThanOrEqual => 4,
        Operator.GreaterThan => 4,
        Operator.NotEqual => 4,
        Operator.BitwiseOr => 5,
        Operator.BitwiseXor => 6,
        Operator.BitwiseAnd => 7,
        Operator.BitShiftLeft => 8,
        Operator.BitShiftRight => 8,
        Operator.Add => 9,
        Operator.Subtract => 9,
        Operator.Concatenate => 9,
        Operator.Multiply => 10,
        Operator.Divide => 10,
        Operator.FloorDivide => 10,
        Operator.Modulo => 10,
        Operator.FloorModulo => 10,
        Operator.Positive => 11,
        Operator.Negative => 11,
        Operator.BitwiseNot => 11,
        Operator.Exponent => 12,
        Operator.SubExpressionSeparator => int.MaxValue,
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