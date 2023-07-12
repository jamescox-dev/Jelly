namespace Jelly.Runtime;

using Jelly.Evaluator;

public class Env : IEnv
{
    public IParser Parser { get; }

    public IEvaluator Evaluator { get; }

    public IScope GlobalScope { get; }

    public IScope CurrentScope { get; private set; }

    internal Env(IScope globalScope, IParser parser, IEvaluator evaluator)
    {
        Parser = parser;
        Evaluator = evaluator;
        GlobalScope = globalScope;
        CurrentScope = GlobalScope;
    }

    internal Env(IParser parser, IEvaluator evaluator) : this(new Scope(), parser, evaluator) {}

    public Env(IScope globalScope) : this(globalScope, new ScriptParser(), new Evaluator()) {}

    public Env() : this(new Scope()) {}

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