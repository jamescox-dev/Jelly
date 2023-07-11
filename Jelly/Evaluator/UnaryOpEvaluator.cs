namespace Jelly.Evaluator;

internal class UnaryOpEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictionaryValue node)
    {
        var op = node.GetString(Keywords.Op);
        var a = env.Evaluate(node.GetNode(Keywords.A));

        if (op == "pos")
        {
            return a.ToDouble().ToValue();
        }
        if (op == "neg")
        {
            return (-a.ToDouble()).ToValue();
        }
        if (op == "not")
        {
            return (!a.ToBool()).ToValue();
        }
        if (op == "bitnot")
        {
            return (~ToInt32(a.ToDouble())).ToValue();
        }

        throw Error.BuildValue("Invalid unary operator.");
    }

    static int ToInt32(double d) => (int) (uint) d;
}