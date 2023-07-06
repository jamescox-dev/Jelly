namespace Jelly.Experimental;

using Jelly.Commands.ArgParsers;

public class MathLibrary : ILibrary
{
    static readonly StringValue Amount = new("amount");
    readonly IArgParser IncArgParser = new StandardArgParser(new Arg("variable"), new OptArg("amount", Node.Literal(1)));

    public void LoadIntoScope(IScope scope)
    {
        var typeMarshaller = new TypeMarshaller();

        scope.DefineCommand("min", new WrappedCommand(MinCmd, typeMarshaller));
        scope.DefineCommand("max", new WrappedCommand(MaxCmd, typeMarshaller));
        scope.DefineCommand("inc", new ArgParsedMacro("inc", IncArgParser, IncMacro));
    }

    public double MinCmd(params double[] numbers)
    {
        if (numbers.Length == 0)
        {
            return 0.0;
        }
        return numbers.Min();
    }

    public double MaxCmd(params double[] numbers)
    {
        if (numbers.Length == 0)
        {
            return 0.0;
        }
        return numbers.Max();
    }

    public Value IncMacro(IEnvironment env, DictionaryValue args)
    {
        var name = env.Evaluate(Node.ToLiteralIfVariable(args.GetNode(Keywords.Variable))).ToString();
        var amount = args.GetNode(Amount);
        return Node.Assignment(name, Node.BinOp("add", Node.Variable(name), amount));
    }
}