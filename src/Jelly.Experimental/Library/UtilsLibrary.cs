namespace Jelly.Experimental.Library;

public class UtilsLibrary : ILibrary
{
    static readonly StrValue Amount = new("amount");
    static readonly IArgParser IncArgParser = new StandardArgParser(new Arg("variable"), new OptArg("amount", Node.Literal(1)));

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

    public static bool IsNumCmd(double value)
    {
        return !double.IsNaN(value);
    }

    public static double NumConvertCmd(double value)
    {
        return value;
    }

    public static bool BoolConvertCmd(bool value)
    {
        return value;
    }

    public static Value IncMacro(IEnv env, DictValue args)
    {
        var name = env.Evaluate(Node.ToLiteralIfVariable(args.GetNode(Keywords.Variable))).ToString();
        var amount = args.GetNode(Amount);
        return Node.Assignment(name, Node.BinOp("add", Node.Variable(name), amount));
    }

    public static Value DecMacro(IEnv env, DictValue args)
    {
        var name = env.Evaluate(Node.ToLiteralIfVariable(args.GetNode(Keywords.Variable))).ToString();
        var amount = args.GetNode(Amount);
        return Node.Assignment(name, Node.BinOp("sub", Node.Variable(name), amount));
    }
}