namespace Jelly.Evaluator;

using Jelly.Values;

internal class BinOpEvaluator : IEvaluator
{
    static readonly StringValue OpKeyword = new StringValue("op");
    static readonly StringValue AKeyword = new StringValue("a");
    static readonly StringValue BKeyword = new StringValue("b");

    Dictionary<string, Func<double, double, double>> _arithmaticOpFuncs = new();
    Dictionary<string, Func<double, double, bool>> _numericComparisonOpFuncs = new();

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
        _numericComparisonOpFuncs.Add("gt", Gt);
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

        throw new NotImplementedException();
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

    static bool Gt(double a, double b) => a > b;

    static void EvaluateOperandsAsNumbers(IScope scope, DictionaryValue node, IEvaluator rootEvaluator, out double a, out double b)
    {
        a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue()).ToDouble();
        b = rootEvaluator.Evaluate(scope, node[BKeyword].ToDictionaryValue()).ToDouble();
    }
}