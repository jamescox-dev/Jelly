namespace Jelly.Runtime;

public interface IEnv
{
    IParser Parser { get; }
    IEvaluator Evaluator { get; }

    // TODO:  Move standard input/output into here.

    // TODO:  Add debugging and memory/speed limiting.

    IScope GlobalScope { get; }
    IScope CurrentScope { get; }

    IScope PushScope();
    IScope PopScope();
    Value RunInNestedScope(Func<Value> action);

    Value Evaluate(string source);
    Value Evaluate(DictionaryValue node);
}