namespace Jelly.Ast.Tests;

[TestFixture]
public class OperatorTests
{
    [Test]
    public void WhenAnOperatorIsNotAKnownValueItsNameIsTheEmptyString()
    {
        var name = ((Operator)(-1)).GetName();

        name.Should().BeEmpty();
    }

    [TestCase(Operator.Or, "or")]
    [TestCase(Operator.OrElse, "orelse")]
    [TestCase(Operator.And, "and")]
    [TestCase(Operator.AndThen, "andthen")]
    [TestCase(Operator.Not, "not")]
    [TestCase(Operator.StrEqual, "streq")]
    [TestCase(Operator.StrNotEqual, "strne")]
    [TestCase(Operator.LessThan, "lt")]
    [TestCase(Operator.LessThanOrEqual, "lte")]
    [TestCase(Operator.Equal, "eq")]
    [TestCase(Operator.GreaterThanOrEqual, "gte")]
    [TestCase(Operator.GreaterThan, "gt")]
    [TestCase(Operator.NotEqual, "ne")]
    [TestCase(Operator.BitwiseOr, "bitor")]
    [TestCase(Operator.BitwiseXor, "bitxor")]
    [TestCase(Operator.BitwiseAnd, "bitand")]
    [TestCase(Operator.BitShiftLeft, "lshift")]
    [TestCase(Operator.BitShiftRight, "rshift")]
    [TestCase(Operator.Add, "add")]
    [TestCase(Operator.Subtract, "sub")]
    [TestCase(Operator.Concatenate, "cat")]
    [TestCase(Operator.Multiply, "mul")]
    [TestCase(Operator.Divide, "div")]
    [TestCase(Operator.FloorDivide, "floordiv")]
    [TestCase(Operator.Modulo, "mod")]
    [TestCase(Operator.FloorModulo, "floormod")]
    [TestCase(Operator.Positive, "pos")]
    [TestCase(Operator.Negative, "neg")]
    [TestCase(Operator.BitwiseNot, "bitnot")]
    [TestCase(Operator.Exponent, "exp")]
    [TestCase(Operator.SubExpressionSeparator, "sep")]
    public void TheCorrectNameOfAnOperatorIsReturned(Operator op, string expectedName)
    {
        var name = op.GetName();

        name.Should().Be(expectedName);
    }
}