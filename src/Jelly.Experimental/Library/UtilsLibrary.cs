namespace Jelly.Experimental.Library;

public class UtilsLibrary : ILibrary
{
    static readonly StrValue Amount = new("amount");
    readonly IArgParser IncArgParser = new StandardArgParser(new Arg("variable"), new OptArg("amount", Node.Literal(1)));

    public void LoadIntoScope(IScope scope)
    {
        var typeMarshaller = new TypeMarshaller();

        scope.DefineCommand("num?", new WrappedCommand(IsNumCmd, typeMarshaller));
        scope.DefineCommand("num", new WrappedCommand(NumConvertCmd, typeMarshaller));

        // TODO:  num n finite?
        // TODO:  num n infinite?

        scope.DefineCommand("bool", new WrappedCommand(BoolConvertCmd, typeMarshaller));

        scope.DefineCommand("inc", new ArgParsedMacro("inc", IncArgParser, IncMacro));
        scope.DefineCommand("dec", new ArgParsedMacro("dec", IncArgParser, DecMacro));
    }

    public bool IsNumCmd(double value)
    {
        return !double.IsNaN(value);
    }

    public double NumConvertCmd(double value)
    {
        return value;
    }

    public bool BoolConvertCmd(bool value)
    {
        return value;
    }

    public Value IncMacro(IEnv env, DictValue args)
    {
        var name = env.Evaluate(Node.ToLiteralIfVariable(args.GetNode(Keywords.Variable))).ToString();
        var amount = args.GetNode(Amount);
        return Node.Assignment(name, Node.BinOp("add", Node.Variable(name), amount));
    }

    public Value DecMacro(IEnv env, DictValue args)
    {
        var name = env.Evaluate(Node.ToLiteralIfVariable(args.GetNode(Keywords.Variable))).ToString();
        var amount = args.GetNode(Amount);
        return Node.Assignment(name, Node.BinOp("sub", Node.Variable(name), amount));
    }
}