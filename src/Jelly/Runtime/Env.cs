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

    public DictionaryValue? Parse(string source) => Parser.Parse(new Scanner(source));

    public Value Evaluate(string source)
    {
        var node = Parse(source);
        if (node is not null)
        {
            return Evaluate(node);
        }
        return Value.Empty;
    }

    public Value Evaluate(DictionaryValue node)
    {
        try
        {
            return Evaluator.Evaluate(this, node);
        }
        catch (Error error)
        {
            if (error.StartPosition < 0 && error.EndPosition < 0)
            {
                var pos = Node.GetPosition(node);
                if (pos is not null)
                {
                    error.EndPosition = (int)pos[Keywords.End].ToDouble();
                    error.StartPosition = (int)pos[Keywords.Start].ToDouble();
                }
            }
            throw error;
        }
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