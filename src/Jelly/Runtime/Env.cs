namespace Jelly.Runtime;

using Jelly.Evaluator;

public class Env : IEnv
{
    public IParser Parser { get; }

    public IEvaluator Evaluator { get; }

    public EnvHooks Hooks { get; }

    public IScope GlobalScope { get; }

    public IScope CurrentScope { get; private set; }

    internal Env(IScope globalScope, EnvHooks hooks, IParser parser, IEvaluator evaluator)
    {
        Parser = parser;
        Evaluator = evaluator;
        Hooks = hooks;
        GlobalScope = globalScope;
        CurrentScope = GlobalScope;
    }

    internal Env(IParser parser, IEvaluator evaluator) : this(new Scope(), new EnvHooks(), parser, evaluator) { }

    public Env(IScope globalScope, EnvHooks hooks) : this(globalScope, hooks, new ScriptParser(), new Evaluator()) { }

    public Env(IScope globalScope) : this(globalScope, new EnvHooks()) { }

    public Env(EnvHooks hooks) : this(new Scope(), hooks) { }

    public Env() : this(new Scope()) { }

    public DictValue? Parse(string source) => Parser.Parse(new Scanner(source));

    public Value Evaluate(string source)
    {
        var node = Parse(source);
        if (node is not null)
        {
            return Evaluate(node);
        }
        return Value.Empty;
    }

    public Value Evaluate(DictValue node)
    {
        try
        {
            Value result = null!;
            Error.RethrowUnhandledClrExceptionsAsJellyErrors(() => 
            {
                Hooks.OnEvaluate?.Invoke(node);
                result = Evaluator.Evaluate(this, node);
            });
            return result;
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
            throw;
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