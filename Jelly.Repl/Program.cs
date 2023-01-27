namespace Jelly.Repl;

using Jelly.Commands;
using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Library;
using Jelly.Parser;
using Jelly.Values;

public class Program
{
    public static void Main(string[] args)
    {
        var config = new DefaultParserConfig();
        var parser = new ScriptParser();
        var global = new Scope();
        
        var coreLib = new CoreLibrary();
        coreLib.LoadIntoScope(global);
        
        global.DefineCommand("print", new SimpleCommand(Print));
        global.DefineCommand("+", new SimpleCommand(Add));
        global.DefineCommand("*", new SimpleCommand(Mul));
        global.DefineCommand(">", new SimpleCommand(Gt));
        global.DefineCommand("<", new SimpleCommand(Lt));
        global.DefineCommand("=", new SimpleCommand(Eq));
        global.DefineCommand("!=", new SimpleCommand(Neq));
        
        if (args.Length == 1)
        {
            using var reader = new StreamReader(args[0]);
            var source = reader.ReadToEnd();
            var position = 0;
            var script = parser.Parse(source, ref position, config);
            if (script is not null)
            {
                var result = Evaluator.Shared.Evaluate(global, script);
                if (result.ToString().Length > 0)
                {
                    Console.WriteLine(result);
                }
            }
        }
        else
        {
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

    static Value Print(IScope scope, ListValue args)
    {
        Console.WriteLine(string.Join(" ", args));
        return Value.Empty;
    }

    static Value Lt(IScope scope, ListValue args)
    {
        return (args[0].ToDouble() < args[1].ToDouble() ? "1" : "0").ToValue();
    }

    static Value Gt(IScope scope, ListValue args)
    {
        return (args[0].ToDouble() > args[1].ToDouble() ? "1" : "0").ToValue();
    }

    static Value Eq(IScope scope, ListValue args)
    {
        return (args[0].ToDouble() == args[1].ToDouble() ? "1" : "0").ToValue();
    }

    static Value Neq(IScope scope, ListValue args)
    {
        return (args[0].ToDouble() != args[1].ToDouble() ? "1" : "0").ToValue();
    }

    static Value Add(IScope scope, ListValue args)
    {
        return args.Select(a => a.ToDouble()).Sum().ToString().ToValue();
    }

    static Value Mul(IScope scope, ListValue args)
    {
        return args.Select(a => a.ToDouble()).Aggregate(1.0, (acc, a) => acc * a).ToString().ToValue();
    }
}


