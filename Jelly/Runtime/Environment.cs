namespace Jelly.Runtime;

using Jelly.Evaluator;

public class Environment : IEnvironment
{
    public IParser Parser { get; }

    public IEvaluator Evaluator { get; }

    public IScope GlobalScope { get; }

    public IScope CurrentScope { get; private set; }

    internal Environment(IParser parser, IEvaluator evaluator)
    {
        Parser = parser;
        Evaluator = evaluator;
        GlobalScope = new Scope();
        CurrentScope = GlobalScope;
    }

    public Environment() : this(new ScriptParser(), new Evaluator()) {}

    public Value Evaluate(string source)
    {
        var node = Parser.Parse(new Scanner(source));
        if (node is not null)
        {
            return Evaluate(node);
        }
        return Value.Empty;
    }

    public Value Evaluate(DictionaryValue node)
    {
        return Evaluator.Evaluate(this, node);
    }

    public IScope PopScope()
    {
        if (CurrentScope.OuterScope is not null)
        {
            CurrentScope = CurrentScope.OuterScope;
            return CurrentScope;
        }
        throw new Error("/error/stack/underflow", "Stack underflow.");
    }

    public IScope PushScope()
    {
        CurrentScope = new Scope(CurrentScope);
        return CurrentScope;
    }

    public Value RunInNestedScope(Func<Value> action)
    {
        PushScope();
        try
        {
            return action();
        }
        finally
        {
            PopScope();
        }
    }
}