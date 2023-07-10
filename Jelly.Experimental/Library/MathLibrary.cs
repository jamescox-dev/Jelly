namespace Jelly.Experimental.Library;

using Jelly.Commands.ArgParsers;

public class MathLibrary : ILibrary
{
    static readonly StringValue Amount = new("amount");
    readonly IArgParser IncArgParser = new StandardArgParser(new Arg("variable"), new OptArg("amount", Node.Literal(1)));

    public void LoadIntoScope(IScope scope)
    {
        var typeMarshaller = new TypeMarshaller();

        scope.DefineCommand("inc", new ArgParsedMacro("inc", IncArgParser, IncMacro));
    }

    public Value IncMacro(IEnvironment env, DictionaryValue args)
    {
        var name = env.Evaluate(Node.ToLiteralIfVariable(args.GetNode(Keywords.Variable))).ToString();
        var amount = args.GetNode(Amount);
        return Node.Assignment(name, Node.BinOp("add", Node.Variable(name), amount));
    }

    // TODO:  clamp
    // TODO:  sign
    // TODO:  abs
    // TODO:  acos
    // TODO:  acosh
    // TODO:  cos
    // TODO:  cosh
    // TODO:  asin
    // TODO:  asinh
    // TODO:  sin
    // TODO:  sinh
    // TODO:  atan
    // TODO:  atan2
    // TODO:  atanh
    // TODO:  tan
    // TODO:  tanh
    // TODO:  exp
    // TODO:  pow
    // TODO:  log
    // TODO:  log2
    // TODO:  log10
    // TODO:  sqrt
    // TODO:  cbrt
    // TODO:  floor
    // TODO:  round
    // TODO:  ceil
    // TODO:  trunc
}