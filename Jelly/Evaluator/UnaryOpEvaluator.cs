namespace Jelly.Evaluator;

using Jelly.Errors;
using Jelly.Values;

internal class UnaryOpEvaluator : IEvaluator
{
    static readonly StringValue OpKeyword = new StringValue("op");
    static readonly StringValue AKeyword = new StringValue("a");

    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        if (node[OpKeyword].ToString() == "pos")
        {
            var a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue(), rootEvaluator);
            return a.ToDouble().ToValue();
        }
        if (node[OpKeyword].ToString() == "neg")
        {
            var a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue(), rootEvaluator);
            return (-a.ToDouble()).ToValue();
        }
        if (node[OpKeyword].ToString() == "not")
        {
            var a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue(), rootEvaluator);
            return (!a.ToBool()).ToValue();
        }
        if (node[OpKeyword].ToString() == "bitnot")
        {
            var a = rootEvaluator.Evaluate(scope, node[AKeyword].ToDictionaryValue(), rootEvaluator);
            return (~ToInt32(a.ToDouble())).ToValue();
        }

        throw Error.Value("Invalid unary operator.");
    }

    static int ToInt32(double d) => (int) (uint) d;
}