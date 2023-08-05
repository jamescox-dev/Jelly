namespace Jelly.Evaluator;

public class ForRangeEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        var iteratorName = env.Evaluate(node.GetNode(Keywords.It)).ToString();
        var start = env.Evaluate(node.GetNode(Keywords.Start)).ToDouble();
        var end = env.Evaluate(node.GetNode(Keywords.End)).ToDouble();
        var step = GetStep(env, node, start, end);
        var body = node.GetNode(Keywords.Body);

        AssertStartEndAndStepValid(start, end, step);
        return RunLoop(env, iteratorName, start, end, step, body);
    }

    static double GetStep(IEnv env, DictValue node, double start, double end)
    {
        if (node.ContainsKey(Keywords.Step))
        {
            return env.Evaluate(node.GetNode(Keywords.Step)).ToDouble();
        }
        var step = Math.Sign(end - start);
        return step != 0 ? step : 1;
    }

    static Value RunLoop(
        IEnv env, string iteratorName, double start, double end, double step, DictValue body)
    {
        var result = Value.Empty;
        for (var i = start; IndexIsBeforeOrAtEnd(i, start, end); i += step)
        {
            PushLoopBodyScope(env, iteratorName, i);
            try
            {
                result = env.Evaluate(body);
            }
            catch (Break)
            {
                result = Value.Empty;
                break;
            }
            catch (Continue)
            {
                continue;
            }
            finally
            {
                env.PopScope();
            }
        }
        return result;
    }

    static void AssertStartEndAndStepValid(double start, double end, double step)
    {
        if (step == 0)
        {
            throw Error.Arg("step can not be zero.");
        }
        if (start < end && step < 0)
        {
            throw Error.Arg("step must be positive when start is less than end.");
        }
        if (start > end && step > 0)
        {
            throw Error.Arg("step must be negative when start is greater than end.");
        }
    }

    static bool IndexIsBeforeOrAtEnd(double i, double start, double end)
    {
        if (start == end)
        {
            return i == end;
        }
        return (start < end) ? i <= end : i >= end;
    }

    static void PushLoopBodyScope(IEnv env, string iteratorName, double value)
    {
        env.PushScope();
        env.CurrentScope.DefineVariable(iteratorName, value.ToValue());
    }
}