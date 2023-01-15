namespace Jelly.Interpreter;

public class LiteralInterpreter : IInterpreter
{
    static readonly StringValue ValueKey = new StringValue("value");

    public Value Evaluate(DictionaryValue node, IInterpreter interpreter) =>
        node[ValueKey];
}