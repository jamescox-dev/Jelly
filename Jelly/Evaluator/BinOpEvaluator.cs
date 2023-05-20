namespace Jelly.Evaluator;

internal class BinOpEvaluator : IEvaluator
{
    static readonly StringValue OpKeyword = new StringValue("op");
    static readonly StringValue AKeyword = new StringValue("a");
    static readonly StringValue BKeyword = new StringValue("b");

    Dictionary<string, Func<double, double, double>> _arithmaticOpFuncs = new();
    Dictionary<string, Func<double, double, bool>> _numericComparisonOpFuncs = new();
    Dictionary<string, Func<int, int, int>> _bitwiseOpFuncs = new();

    public BinOpEvaluator()
    {
        _arithmaticOpFuncs.Add("add", Add);
        _arithmaticOpFuncs.Add("sub", Sub);
        _arithmaticOpFuncs.Add("mul", Mul);
        _arithmaticOpFuncs.Add("div", Div);
        _arithmaticOpFuncs.Add("floordiv", FloorDiv);
        _arithmaticOpFuncs.Add("mod", Mod);
        _arithmaticOpFuncs.Add("floormod", FloorMod);
        _arithmaticOpFuncs.Add("exp", Exp);

        _numericComparisonOpFuncs.Add("lt", Lt);
        _numericComparisonOpFuncs.Add("lte", LtEq);
        _numericComparisonOpFuncs.Add("eq", Eq);
        _numericComparisonOpFuncs.Add("gte", GtEq);
        _numericComparisonOpFuncs.Add("gt", Gt);
        _numericComparisonOpFuncs.Add("ne", Neq);

        _bitwiseOpFuncs.Add("bitor", BitwiseOr);
        _bitwiseOpFuncs.Add("bitand", BitwiseAnd);
        _bitwiseOpFuncs.Add("bitxor", BitwiseXor);
        _bitwiseOpFuncs.Add("lshift", LeftShift);
        _bitwiseOpFuncs.Add("rshift", RightShift);
    }

    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        throw new NotImplementedException();
    }

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        if (_arithmaticOpFuncs.TryGetValue(node[OpKeyword].ToString(), out var arithOp))
        {
            EvaluateOperandsAsNumbers(scope, node, rootEvaluator, out var a, out var b);
            return new NumberValue(arithOp(a, b));
        }
        if (_numericComparisonOpFuncs.TryGetValue(node[OpKeyword].ToString(), out var numCompOp))
        {
            EvaluateOperandsAsNumbers(scope, node, rootEvaluator, out var a, out var b);
            return numCompOp(a, b).ToValue();
        }
        if (_bitwiseOpFuncs.TryGetValue(node[OpKeyword].ToString(), out var bitwiseOp))
        {
            EvaluateOperandsAsInt32s(scope, node, rootEvaluator, out var a, out var b);
            return bitwiseOp(a, b).ToValue();
        }
        var op = node[OpKeyword].ToString();
        if (op == "cat")
        {
            EvaluateOperandsAsStrings(scope, node, rootEvaluator, out var a, out var b);
            return (a + b).ToValue();
        }
        if (op == "and")
        {
            return And(scope, node, rootEvaluator);
        }
        if (op == "andthen")
        {
            return AndThen(scope, node, rootEvaluator);
        }
        if (op == "or")
        {
            return Or(scope, node, rootEvaluator);
        }
        if (op == "orelse")
        {
            return OrElse(scope, node, rootEvaluator);
        }

        throw Error.BuildValue("Invalid binary operator.");
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

    static int BitwiseOr(int a, int b) => a | b;

    static int BitwiseAnd(int a, int b) => a & b;

    static int BitwiseXor(int a, int b) => (int)((uint)a ^ (uint)b);

    static int LeftShift(int a, int b) => a << (b & 0x1f);

    static int RightShift(int a, int b) => a >> (b & 0x1f);

    static Value And(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        EvaluateOperandsAsBooleans(scope, node, rootEvaluator, out var a, out var b);
        return (a && b).ToValue();
    }

    static Value AndThen(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue(), rootEvaluator);
        if (!a.ToBool())
        {
            return a;
        }
        return rootEvaluator.Evaluate(scope, node[BKeyword].ToDictionaryValue(), rootEvaluator);
    }

    static Value Or(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        EvaluateOperandsAsBooleans(scope, node, rootEvaluator, out var a, out var b);
        return (a || b).ToValue();
    }

    static Value OrElse(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue(), rootEvaluator);
        if (a.ToBool())
        {
            return a;
        }
        return rootEvaluator.Evaluate(scope, node[BKeyword].ToDictionaryValue(), rootEvaluator);
    }

    static void EvaluateOperandsAsNumbers(IScope scope, DictionaryValue node, IEvaluator rootEvaluator, out double a, out double b)
    {
        a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue()).ToDouble();
        b = rootEvaluator.Evaluate(scope, node[BKeyword].ToDictionaryValue()).ToDouble();
    }

    static void EvaluateOperandsAsInt32s(IScope scope, DictionaryValue node, IEvaluator rootEvaluator, out int a, out int b)
    {
        EvaluateOperandsAsNumbers(scope, node, rootEvaluator, out var aDbl, out var bDbl);
        a = (int) (uint) aDbl;
        b = (int) (uint) bDbl;
    }

    static void EvaluateOperandsAsBooleans(IScope scope, DictionaryValue node, IEvaluator rootEvaluator, out bool a, out bool b)
    {
        a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue()).ToBool();
        b = rootEvaluator.Evaluate(scope, node[BKeyword].ToDictionaryValue()).ToBool();
    }

    static void EvaluateOperandsAsStrings(IScope scope, DictionaryValue node, IEvaluator rootEvaluator, out string a, out string b)
    {
        a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue()).ToString();
        b = rootEvaluator.Evaluate(scope, node[BKeyword].ToDictionaryValue()).ToString();
    }
}