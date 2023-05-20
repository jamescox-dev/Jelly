namespace Jelly.Runtime;

public interface IEnvironment
{
    IParser Parser { get; }
    IEvaluator Evaluator { get; }

    // TODO:  Move standard input/output into here.

    IScope GlobalScope { get; }
    IScope CurrentScope { get; }

    IScope PushScope();
    IScope PopScope();

    Value Evaluate(string source);
    Value Evaluate(DictionaryValue node);
}