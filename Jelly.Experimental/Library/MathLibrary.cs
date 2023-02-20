namespace Jelly.Experimental;

using Jelly;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Library;
using Jelly.Values;

public class MathLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("+", new SimpleCommand(CmdAdd));
        scope.DefineCommand("-", new SimpleCommand(CmdSub));
        scope.DefineCommand("*", new SimpleCommand(CmdMul));
        scope.DefineCommand("/", new SimpleCommand(CmdDiv));
        // TODO:  //
        // TODO:  %
        // TODO:  %%
        // TODO:  **

        // TODO: ++

        scope.DefineCommand("=", new SimpleCommand(CmdEq));
        scope.DefineCommand("!=", new SimpleCommand(CmdNeq));
        scope.DefineCommand("<=", new SimpleCommand(CmdLtEq));
        scope.DefineCommand(">=", new SimpleCommand(CmdGtEq));
        scope.DefineCommand("<", new SimpleCommand(CmdLt));
        scope.DefineCommand(">", new SimpleCommand(CmdGt));
    
        // TODO:  eq, neq, lt, lte, gt, gte
        // TODO:  ieq, ineq, ilt, ilte, igt, igte
        
        scope.DefineCommand("and", new SimpleCommand(CmdAnd));
        scope.DefineCommand("andthen", new SimpleMacro(CmdAndThen));
        // TODO:  or
        // TODO:  andthen
        // TODO:  orelse
        // TODO:  not
    }

    public Value CmdAdd(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args);
        return new NumberValue(ns.Sum());
    }

    public Value CmdSub(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args).ToList();
        if (ns.Count == 0)
        {
            throw Error.Arg("Expected number.");
        }
        if (ns.Count == 1)
        {
            return new NumberValue(-ns[0]);
        }
        return new NumberValue(ns.Skip(1).Aggregate(ns[0], (acc, n) => acc - n));
    }

    public Value CmdMul(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args).ToList();
        return new NumberValue(ns.Aggregate(1.0, (acc, n) => acc * n));
    }

    public Value CmdDiv(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args).ToList();
        if (ns.Count == 0)
        {
            throw Error.Arg("Expected number.");
        }
        if (ns.Count == 1)
        {
            return new NumberValue(1.0 / ns[0]);
        }
        return new NumberValue(ns.Skip(1).Aggregate(ns[0], (acc, n) => acc / n));
    }

    public Value CmdEq(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args).ToList();
        if (ns.Count <= 1)
        {
            return new NumberValue(1);
        }
        return new NumberValue(ns.All(n => n == ns[0]) ? 1.0 : 0.0);
    }

    public Value CmdNeq(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args).ToList();
        if (ns.Count <= 1)
        {
            return new NumberValue(1);
        }
        return new NumberValue(ns.Any(n => n != ns[0]) ? 1.0 : 0.0);
    }

    public Value CmdLtEq(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args).ToList();
        if (args.Count <= 1)
        {
            return new NumberValue(1);
        }
        return new NumberValue(ns.OrderBy(n => n).SequenceEqual(ns) ? 1.0 : 0.0);
    }

    public Value CmdGtEq(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args).ToList();
        if (args.Count <= 1)
        {
            return new NumberValue(1);
        }
        return new NumberValue(ns.OrderByDescending(n => n).SequenceEqual(ns) ? 1.0 : 0.0);
    }

    public Value CmdLt(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args).ToList();
        if (args.Count <= 1)
        {
            return new NumberValue(1);
        }
        var prev = ns[0];
        return new NumberValue(ns.Skip(1).All(n => { var res = prev < n; prev = n; return res; }) ? 1.0 : 0.0);
    }

    public Value CmdGt(IScope scope, ListValue args)
    {
        var ns = ListAsDoubles(args).ToList();
        if (args.Count <= 1)
        {
            return new NumberValue(1);
        }
        var prev = ns[0];
        return new NumberValue(ns.Skip(1).All(n => { var res = prev > n; prev = n; return res; }) ? 1.0 : 0.0);
    }

    public IEnumerable<double> ListAsDoubles(ListValue list)
    {
        var expect = false;
        var sign = 1;
        foreach (var item in list)
        {
            if (item.Equals("-".ToValue()))
            {
                expect = true;
                sign = -1;
            }
            else if (item.Equals("+".ToValue()))
            {
                expect = true;
            }
            else
            {
                expect = false;
                yield return item.ToDouble() * sign;
                sign = 1;
            }
        }

        if (expect)
        {
            throw Error.Arg("Expected number.");
        }
    }

    public Value CmdAnd(IScope scope, ListValue args)
    {
        foreach (var arg in args)
        {
            if (arg.ToDouble() == 0.0)
            {
                return arg;
            }
        }

        return args.Count == 0 ? NumberValue.One : args[args.Count - 1];
    }

    public Value CmdAndThen(IScope scope, ListValue args)
    {
        var evaluator = new Evaluator();

        Value value = NumberValue.One;
        foreach (var arg in args)
        {
            value = evaluator.Evaluate(scope, arg.ToDictionaryValue(), evaluator);
            if (value.ToDouble() == 0.0)
            {
                return value;
            }
        }

        return value;
    }
}