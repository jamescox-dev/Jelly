namespace Jelly.Interpreter;

public class LiteralInterpreter : IInterpreter
{
    static readonly StringValue ValueKey = new StringValue("value");

    public Value Evaluate(Scope scope, DictionaryValue node, IInterpreter interpreter) =>
        node[ValueKey];
}