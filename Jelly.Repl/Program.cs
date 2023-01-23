namespace Jelly.Repl;

using Jelly.Commands;
using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Parser;
using Jelly.Values;

public class Program
{
    public static void Main()
    {
        var config = new DefaultParserConfig();
        var parser = new ScriptParser();

        var global = new Scope();
        global.DefineVariable("cmd", "print".ToValue());
        global.DefineVariable("msg", "jello, world".ToValue());
        global.DefineCommand("print", new PrintCommand());
        global.DefineCommand("repeat", new RepeatCommand());
        global.DefineVariable("message", "jello, world".ToValue());

        for (;;)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (input is not null)
            {
                try
                {
                    var position = 0;
                    var command = parser.Parse(input, ref position, config);
                    if (command is not null)
                    {
                        var result = Evaluator.Shared.Evaluate(global, command);
                        if (result.ToString().Length > 0)
                        {
                            Console.WriteLine(result);
                        }
                    }
                }
                catch (Error error)
                {
                    Console.Error.WriteLine($"{error.Type}:  {error.Message}");
                }
                catch (Exception unexpected)
                {
                    Console.Error.WriteLine($"Unexpected error:  {unexpected.ToString()}");
                }
            }
        }
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