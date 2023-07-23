namespace Jelly.Evaluator;

internal class BinOpEvaluator : IEvaluator
{
    delegate double ArithmeticOp(double a, double b);
    delegate bool ComparisonOp(double a, double b);
    delegate bool StringOp(Value a, Value b);
    delegate int BitwiseOp(int a, int b);

    readonly Dictionary<string, ArithmeticOp> _arithmeticOps = new();
    readonly Dictionary<string, ComparisonOp> _comparisonOps = new();
    readonly Dictionary<string, StringOp> _stringOps = new();
    readonly Dictionary<string, BitwiseOp> _bitwiseOps = new();

    public BinOpEvaluator()
    {
        _arithmeticOps.Add("add", Add);
        _arithmeticOps.Add("sub", Sub);
        _arithmeticOps.Add("mul", Mul);
        _arithmeticOps.Add("div", Div);
        _arithmeticOps.Add("floordiv", FloorDiv);
        _arithmeticOps.Add("mod", Mod);
        _arithmeticOps.Add("floormod", FloorMod);
        _arithmeticOps.Add("exp", Exp);

        _comparisonOps.Add("lt", Lt);
        _comparisonOps.Add("lte", LtEq);
        _comparisonOps.Add("eq", Eq);
        _comparisonOps.Add("gte", GtEq);
        _comparisonOps.Add("gt", Gt);
        _comparisonOps.Add("ne", Neq);

        _stringOps.Add("strne", StrNotEqual);
        _stringOps.Add("streq", StrEqual);

        _bitwiseOps.Add("bitor", BitwiseOr);
        _bitwiseOps.Add("bitand", BitwiseAnd);
        _bitwiseOps.Add("bitxor", BitwiseXor);
        _bitwiseOps.Add("lshift", LeftShift);
        _bitwiseOps.Add("rshift", RightShift);
    }

    public Value Evaluate(IEnv env, DictValue node)
    {
        var op = node.GetString(Keywords.Op);

        if (_arithmeticOps.TryGetValue(op, out var arithOp))
        {
            EvaluateOperandsAsNumbers(env, node, out var a, out var b);
            return new NumberValue(arithOp(a, b));
        }
        if (_comparisonOps.TryGetValue(op, out var numCompOp))
        {
            EvaluateOperandsAsNumbers(env, node, out var a, out var b);
            return numCompOp(a, b).ToValue();
        }
        if (_stringOps.TryGetValue(op, out var stringOp))
        {
            return stringOp(env.Evaluate(node.GetNode(Keywords.A)), env.Evaluate(node.GetNode(Keywords.B))).ToValue();
        }
        if (_bitwiseOps.TryGetValue(op, out var bitwiseOp))
        {
            EvaluateOperandsAsInt32s(env, node, out var a, out var b);
            return bitwiseOp(a, b).ToValue();
        }
        if (op == "cat")
        {
            EvaluateOperandsAsStrings(env, node, out var a, out var b);
            return (a + b).ToValue();
        }
        if (op == "and")
        {
            return And(env, node);
        }
        if (op == "andthen")
        {
            return AndThen(env, node);
        }
        if (op == "or")
        {
            return Or(env, node);
        }
        if (op == "orelse")
        {
            return OrElse(env, node);
        }

        throw Error.BuildValue("Invalid binary operator.");
    }

    static Value And(IEnv env, DictValue node)
    {
        EvaluateOperandsAsBooleans(env, node, out var a, out var b);
        return (a && b).ToValue();
    }

    static Value AndThen(IEnv env, DictValue node)
    {
        var a = env.Evaluate(node.GetNode(Keywords.A));
        if (!a.ToBool())
        {
            return a;
        }
        return env.Evaluate(node.GetNode(Keywords.B));
    }

    static Value Or(IEnv env, DictValue node)
    {
        EvaluateOperandsAsBooleans(env, node, out var a, out var b);
        return (a || b).ToValue();
    }

    static Value OrElse(IEnv env, DictValue node)
    {
        var a = env.Evaluate(node.GetNode(Keywords.A));
        if (a.ToBool())
        {
            return a;
        }
        return env.Evaluate(node.GetNode(Keywords.B));
    }

    static void EvaluateOperandsAsNumbers(IEnv env, DictValue node, out double a, out double b)
    {
        a = env.Evaluate(node.GetNode(Keywords.A)).ToDouble();
        b = env.Evaluate(node.GetNode(Keywords.B)).ToDouble();
    }

    static void EvaluateOperandsAsInt32s(IEnv env, DictValue node, out int a, out int b)
    {
        EvaluateOperandsAsNumbers(env, node, out var aDbl, out var bDbl);
        a = (int) (uint) aDbl;
        b = (int) (uint) bDbl;
    }

    static void EvaluateOperandsAsBooleans(IEnv env, DictValue node, out bool a, out bool b)
    {
        a = env.Evaluate(node.GetNode(Keywords.A)).ToBool();
        b = env.Evaluate(node.GetNode(Keywords.B)).ToBool();
    }

    static void EvaluateOperandsAsStrings(IEnv env, DictValue node, out string a, out string b)
    {
        a = env.Evaluate(node.GetNode(Keywords.A)).ToString();
        b = env.Evaluate(node.GetNode(Keywords.B)).ToString();
    }

    static double Add(double a, double b) => a + b;

    static double Sub(double a, double b) => a - b;

    static double Mul(double a, double b) => a * b;

    static double Div(double a, double b) => a / b;

    static double FloorDiv(double a, double b) => Math.Floor(a / b);

    static double Mod(double a, double b) => ((a % b) + b) % b;

    static double FloorMod(double a, double b) => Math.Floor(Mod(a, b));

    static double Exp(double a, double b) => Math.Pow(a, b);

    static bool Lt(double a, double b) => a < b;

    static bool LtEq(double a, double b) => a <= b;

    static bool Eq(double a, double b) => a == b;

    static bool GtEq(double a, double b) => a >= b;

    static bool Gt(double a, double b) => a > b;

    static bool Neq(double a, double b) => a != b;

    static bool StrNotEqual(Value a, Value b) => !a.Equals(b);

    static bool StrEqual(Value a, Value b) => a.Equals(b);

    static int BitwiseOr(int a, int b) => a | b;

    static int BitwiseAnd(int a, int b) => a & b;

    static int BitwiseXor(int a, int b) => (int)((uint)a ^ (uint)b);

    static int LeftShift(int a, int b) => a << (b & 0x1f);

    static int RightShift(int a, int b) => a >> (b & 0x1f);
}