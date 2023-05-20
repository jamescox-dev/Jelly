namespace Jelly.Library;

using Jelly.Evaluator;

using System.Collections.Immutable;

public class CoreLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("break", new SimpleMacro(BreakMacro));
        scope.DefineCommand("continue", new SimpleMacro(ContinueMacro));
        scope.DefineCommand("def", new SimpleMacro(DefMacro));
        scope.DefineCommand("for", new SimpleMacro(ForMacro));
        scope.DefineCommand("if", new SimpleMacro(IfMacro));
        scope.DefineCommand("lsdef", new WrappedCommand(LsDefCmd, TypeMarshaller.Shared));
        scope.DefineCommand("lsvar", new WrappedCommand(LsVarCmd, TypeMarshaller.Shared));
        scope.DefineCommand("raise", new SimpleMacro(RaiseMacro));
        scope.DefineCommand("return", new SimpleMacro(ReturnMacro));
        scope.DefineCommand("try", new SimpleMacro(TryMacro));
        scope.DefineCommand("var", new SimpleMacro(VarMacro));
        scope.DefineCommand("while", new SimpleMacro(WhileMacro));
    }

    Value BreakMacro(IScope scope, ListValue args)
    {
        if (args.Count != 0)
        {
            throw Error.Arg($"Unexpected argument '{Evaluator.Shared.Evaluate(scope, args[0].ToDictionaryValue())}'.");
        }

        return Node.Raise(
            Node.Literal("/break/"),
            Node.Literal(Value.Empty),
            Node.Literal(Value.Empty)
        );
    }

    Value ContinueMacro(IScope scope, ListValue args)
    {
        if (args.Count != 0)
        {
            throw Error.Arg($"Unexpected argument '{Evaluator.Shared.Evaluate(scope, args[0].ToDictionaryValue())}'.");
        }

        return Node.Raise(
            Node.Literal("/continue/"),
            Node.Literal(Value.Empty),
            Node.Literal(Value.Empty)
        );
    }

    Value DefMacro(IScope scope, ListValue args)
    {
        if (args.Count == 0)
        {
            throw Error.Arg("Expected 'name'.");
        }
        if (args.Count == 1)
        {
            throw Error.Arg("Expected 'body'.");
        }

        var name = args[0].ToDictionaryValue();
        var body = args[args.Count - 1].ToDictionaryValue();

        var requireArg = true;
        var expectDefault = false;
        var expectEquals = false;
        var argNames = ImmutableList.CreateBuilder<DictionaryValue>();
        var argDefaults = ImmutableList.CreateBuilder<DictionaryValue>();
        foreach (var arg in args.Skip(1).Take(args.Count - 2))
        {
            var argDict = arg.ToDictionaryValue();
            if (expectEquals && !IsKeyword(argDict, "="))
            {
                throw Error.Arg($"Argument '{Evaluator.Shared.Evaluate(scope, argNames[argNames.Count - 1].ToDictionaryValue())}' must have a default value.");
            }
            if (argNames.Count > 0 && IsKeyword(argDict, "=") && !expectDefault && !requireArg)
            {
                expectDefault = true;
                expectEquals = false;
            }
            else
            {
                if (expectDefault)
                {
                    argDefaults.Add(argDict);
                    requireArg = true;
                }
                else
                {
                    argNames.Add(TryConvertVariableToLiteral(argDict));
                    requireArg = false;
                    expectEquals = argDefaults.Count > 0;
                }
                expectDefault = false;
            }
        }

        if (expectEquals)
        {
            throw Error.Arg($"Argument '{Evaluator.Shared.Evaluate(scope, argNames[argNames.Count - 1].ToDictionaryValue())}' must have a default value.");
        }

        DictionaryValue? restArg = null;
        if (expectDefault)
        {
            restArg = argNames[argNames.Count - 1];
            argNames.RemoveAt(argNames.Count - 1);
        }

        return Node.DefineCommand(name, body, new ListValue(argNames.ToImmutable()), new ListValue(argDefaults.ToImmutable()), restArg);
    }

    static DictionaryValue TryConvertVariableToLiteral(Value node)
    {
        var nodeDict = node.ToDictionaryValue();

        return Node.IsVariable(nodeDict) ? Node.Literal(nodeDict[Keywords.Name].ToString()) : nodeDict;
    }

    Value ForMacro(IScope scope, ListValue args)
    {
        // TODO:  for i = 1 to 10 {}, for i = 1 to 10 step 2 {}, for v in list {}, for i v in list, for v of dict {}, for k v of dict {}

        if (args.Count == 0)
        {
            throw Error.Arg("Expected 'iterator'.");
        }
        else if (args.Count == 1)
        {
            throw Error.Arg("Expected 'iterator', or '=', 'in', or 'of' keyword.");
        }
        else if (args.Count >= 2)
        {
            if (IsKeyword(args[1].ToDictionaryValue(), "in"))
            {
                if (args.Count == 2)
                {
                    throw Error.Arg("Expected 'list'.");
                }
                else if (args.Count == 3)
                {
                    throw Error.Arg("Expected 'body'.");
                }
                else if (args.Count > 4)
                {
                    throw Error.Arg($"Unexpected '{Evaluator.Shared.Evaluate(scope, args[4].ToDictionaryValue())}', after 'body'.");
                }
            }
            else if (IsKeyword(args[1].ToDictionaryValue(), "of"))
            {
                if (args.Count == 2)
                {
                    throw Error.Arg("Expected 'dict'.");
                }
                else if (args.Count == 3)
                {
                    throw Error.Arg("Expected 'body'.");
                }
                else if (args.Count > 4)
                {
                    throw Error.Arg($"Unexpected '{Evaluator.Shared.Evaluate(scope, args[4].ToDictionaryValue())}', after 'body'.");
                }
            }
            else if (IsKeyword(args[1].ToDictionaryValue(), "="))
            {
                throw Error.Arg("Expected 'start'.");
            }
        }

        throw new NotImplementedException();
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

        var conditions = new List<DictionaryValue> { args[0].ToDictionaryValue() };
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
                    conditions.Add(arg);
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

        foreach ((var condition, var thenBody) in conditions.Zip(thenBodies).Reverse())
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

    static bool IsKeyword(DictionaryValue word, string keyword)
    {
        return Node.IsLiteral(word)
            && Node.GetLiteralValue(word).ToString()
                .Equals(keyword, StringComparison.InvariantCultureIgnoreCase);
    }

    string[] LsDefCmd(IScope scope, bool localOnly = false)
    {
        var commands = scope.GetCommandNames(localOnly);
        return commands.OrderBy(c => c, StringComparer.InvariantCulture).ToArray();
    }

    string[] LsVarCmd(IScope scope, bool localOnly = false)
    {
        var commands = scope.GetVariableNames(localOnly);
        return commands.OrderBy(c => c, StringComparer.InvariantCulture).ToArray();
    }

    Value RaiseMacro(IScope scope, ListValue args)
    {
        if (args.Count == 0)
        {
            throw Error.Arg("Expected 'type' argument.");
        }
        if (args.Count > 3)
        {
            throw Error.Arg($"Unexpected argument '{Evaluator.Shared.Evaluate(scope, args[3].ToDictionaryValue())}'.");
        }

        var type = args[0].ToDictionaryValue();
        var message = args.Count > 1 ? args[1].ToDictionaryValue() : Node.Literal(Value.Empty);
        var value = args.Count > 2 ? args[2].ToDictionaryValue() : Node.Literal(Value.Empty);

        return Node.Raise(type, message, value);
    }

    Value ReturnMacro(IScope scope, ListValue args)
    {
        var value = Node.Literal(Value.Empty);

        if (args.Count == 1)
        {
            value = args[0].ToDictionaryValue();
        }
        else if (args.Count > 1)
        {
            throw Error.Arg($"Unexpected argument '{Evaluator.Shared.Evaluate(scope, args[1].ToDictionaryValue())}'.");
        }

        return Node.Raise(
            Node.Literal("/return/"),
            Node.Literal(Value.Empty),
            value);
    }

    Value TryMacro(IScope scope, ListValue args)
    {
        if (args.Count == 0)
        {
            throw Error.Arg("Expected 'body' argument.");
        }

        var body = Node.Scope(args[0].ToDictionaryValue());
        var errorHandlers = new List<(DictionaryValue, DictionaryValue)>();
        DictionaryValue? finallyBody = null;
        for (var i = 1; i < args.Count;)
        {
            var arg = args[i].ToDictionaryValue();

            if (IsKeyword(arg, "except"))
            {
                if (finallyBody is not null)
                {
                    throw Error.Arg("Unexpected 'except' argument after 'finally'.");
                }
                if (args.Count <= i + 1)
                {
                    throw Error.Arg("Expected 'error_details' argument.");
                }
                if (args.Count <= i + 2)
                {
                    throw Error.Arg("Expected 'except_body' argument.");
                }
                errorHandlers.Add((args[i + 1].ToDictionaryValue(), Node.Scope(args[i + 2].ToDictionaryValue())));
                i += 3;
            }
            else if (IsKeyword(arg, "finally"))
            {
                if (finallyBody is not null)
                {
                    throw Error.Arg("Unexpected duplicate 'finally' argument.");
                }
                if (args.Count <= i + 1)
                {
                    throw Error.Arg("Expected 'finally_body' argument.");
                }
                finallyBody = Node.Scope(args[i + 1].ToDictionaryValue());
                i += 2;
            }
            else
            {
                throw Error.Arg($"Unexpected '{Evaluator.Shared.Evaluate(scope, arg)}' argument.");
            }
        }

        return Node.Try(
            body,
            finallyBody,
            errorHandlers.ToArray()
        );
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