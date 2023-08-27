

namespace Jelly.Evaluator;

internal class CompositeEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        return new StrValue(string.Join("", GetParts(env, node)));
    }

    static IEnumerable<Value> GetParts(IEnv env, DictValue node)
    {
        return node.GetList(Keywords.Parts)
                    .Select(part => env.Evaluate(part.ToNode()));
    }
}