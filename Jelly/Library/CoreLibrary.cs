namespace Jelly.Library;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Commands;
using Jelly.Values;

public class CoreLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("if", new SimpleMacro(IfMacro));
        scope.DefineCommand("var", new SimpleMacro(VarMacro));
        scope.DefineCommand("while", new SimpleMacro(WhileMacro));
    }

    Value IfMacro(IScope scope, ListValue args)
    {
        if (args.Count == 0)
        {
            throw Error.Arg("Expected 'condition'.");
        }
        if (args.Count == 1)
        {
            throw Error.Arg("Expected 'then_body'.");
        }

        var conditoins = new List<DictionaryValue> { args[0].ToDictionaryValue() };
        var thenBodies = new List<DictionaryValue> { args[1].ToDictionaryValue() };
        DictionaryValue? elseBody = null;

        var expectElse = false;
        var i = 2;
        while (i < args.Count)
        {
            var arg = args[i].ToDictionaryValue();

            if (i % 3 == 0)
            {
                if (!expectElse)
                {
                    conditoins.Add(arg);
                }
                else
                {
                    elseBody = arg;
                }
            }
            else if (i % 3 == 1)
            {
                if (expectElse)
                {
                    throw Error.Arg("Unexpected arguments after 'else_body'.");
                }
                thenBodies.Add(arg);
            }
            else if (i % 3 == 2)
            {
                if (IsKeyword(arg, "else"))
                {
                    expectElse = true;
                }
                else if (IsKeyword(arg, "elif"))
                {
                    if (i + 1 >= args.Count)
                    {
                        throw Error.Arg("Expected 'condition'.");
                    }
                    else if (i + 2 >= args.Count)
                    {
                        throw Error.Arg("Expected 'then_body'.");
                    }
                }
                else
                {
                    throw Error.Arg("Expected 'elif', or 'else' keyword.");
                }
            }

            ++i;
        }

        if (expectElse && elseBody is null)
        {
            throw Error.Arg("Expected 'else_body'.");
        }

        DictionaryValue? ifNode = null;

        foreach ((var condition, var thenBody) in conditoins.Zip(thenBodies).Reverse())
        {
            if (ifNode is null)
            {
                if (elseBody is null)
                {
                    ifNode = Node.If(condition, Node.Scope(thenBody));
                }
                else
                {
                    ifNode = Node.If(condition, Node.Scope(thenBody), Node.Scope(elseBody));
                }
            }
            else
            {
                ifNode = Node.If(condition, Node.Scope(thenBody), ifNode);
            }
        }

        return ifNode!;
    }

    private static bool IsKeyword(DictionaryValue word, string keyword)
    {
        return Node.IsLiteral(word) 
            && Node.GetLiteralValue(word).ToString()
                .Equals(keyword, StringComparison.InvariantCultureIgnoreCase);
    }

    Value VarMacro(IScope scope, ListValue args)
    {
        if (args.Count == 0)
        {
            throw new ArgError("Expected 'varname'.");
        }
        if (args.Count >= 2)
        {
            var keyword = Evaluator.Shared.Evaluate(scope, args[1].ToDictionaryValue()).ToString();
            if (!keyword.Equals("=", StringComparison.InvariantCulture))
            {
                throw new ArgError($"Expected keyword '=', but found '{keyword}'.");
            }
        }
        if (args.Count > 3)
        {
            var unexpected = Evaluator.Shared.Evaluate(scope, args[3].ToDictionaryValue()).ToString();
            throw new ArgError($"Unexpected value '{unexpected}'.");
        }

        var varnameNode = args[0].ToDictionaryValue();
        var isVariable = varnameNode.ContainsKey(Keywords.Type) && varnameNode[Keywords.Type].Equals(Keywords.Variable);
        var varname = isVariable ? varnameNode[Keywords.Name].ToString() : Evaluator.Shared.Evaluate(scope, varnameNode).ToString();

        return Node.DefineVariable(varname, args.Count == 3 ? args[2].ToDictionaryValue() : Node.Literal(Value.Empty));
    }

    Value WhileMacro(IScope scope, ListValue args)
    {
        if (args.Count == 0)
        {
            throw new ArgError("Expected 'condition'.");
        }
        if (args.Count == 1)
        {
            throw new ArgError("Expected 'body'.");
        }
        if (args.Count > 2)
        {
            var unexpected = Evaluator.Shared.Evaluate(scope, args[2].ToDictionaryValue()).ToString();
            throw new ArgError($"Unexpected value '{unexpected}'.");
        }

        return Node.While(args[0].ToDictionaryValue(), Node.Scope(args[1].ToDictionaryValue()));
    }
}