namespace Jelly.Evaluator;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Values;

public class ForRangeEvaluator : IEvaluator
{
    public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator rootEvaluator)
    {
        var iteratorName = rootEvaluator.Evaluate(scope, node[Keywords.It].ToDictionaryValue()).ToString();
        var start = rootEvaluator.Evaluate(scope, node[Keywords.Start].ToDictionaryValue()).ToDouble();
        var end = rootEvaluator.Evaluate(scope, node[Keywords.End].ToDictionaryValue()).ToDouble();
        var step = rootEvaluator.Evaluate(scope, node[Keywords.Step].ToDictionaryValue()).ToDouble();
        var body = node[Keywords.Body].ToDictionaryValue();

        if (step == 0)
        {
            throw Error.Arg("step can not be zero.");
        }
        
        var result = Value.Empty;
        if (step < 0)
        {
            if (start < end)
            {
                throw Error.Arg("step must be positive when start is less than end.");
            }
            for (var i = start; i >= end; i += step)
            {
                var innerScope = new Scope(scope);
                innerScope.DefineVariable(iteratorName, i.ToValue());
                try
                {
                    result = rootEvaluator.Evaluate(innerScope, body);
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
            }
            return result;
        }
        
        if (start > end)
        {
            throw Error.Arg("step must be negative when start is greater than end.");
        }
        for (var i = start; i <= end; i += step)
        {
            var innerScope = new Scope(scope);
            innerScope.DefineVariable(iteratorName, i.ToValue());
            try
            {
                result = rootEvaluator.Evaluate(innerScope, body);
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
        }
        return result;
    }
}