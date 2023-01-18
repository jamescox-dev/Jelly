namespace Jelly.Repl;

using Jelly.Commands;
using Jelly.Evaluator;
using Jelly.Parser;
using Jelly.Values;

public class Program
{
    public static void Main()
    {
        var global = new Scope();
        global.DefineCommand("print", new PrintCommand());
        global.DefineCommand("repeat", new RepeatCommand());
        global.DefineVariable("message", "jello, world".ToValue());

        var b = new NodeBuilder();
        var script = b.Script(
            b.Command(b.Literal("repeat".ToValue()), new ListValue(b.Literal("3".ToValue()),
                b.Command(b.Literal("print".ToValue()), new ListValue(
                    b.Variable("message")
                ))))
        );

        Evaluator.Shared.Evaluate(global, script);
    }
}

public class PrintCommand : ICommand
{
    public bool IsMacro => false;

    public Value Invoke(IScope scope, ListValue args)
    {
        Console.WriteLine(args[0]);
        return Value.Empty;
    }
}

public class RepeatCommand : ICommand
{
    public bool IsMacro => true;

    public Value Invoke(IScope scope, ListValue args)
    {
        var count = int.Parse(Evaluator.Shared.Evaluate(scope, args[0].ToDictionaryValue()).ToString());
        var result = Value.Empty;
        while (count > 0)
        {
            result = Evaluator.Shared.Evaluate(scope, args[1].ToDictionaryValue());
            --count;
        }
        return result;
    }
}