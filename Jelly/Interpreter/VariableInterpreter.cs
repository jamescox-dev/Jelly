namespace Jelly.Interpreter;

public class VariableInterpreter : IInterpreter
{
    static readonly StringValue NameKey = new("name");

    public Value Evaluate(Scope scope, DictionaryValue node, IInterpreter interpreter) =>
        scope.GetVariable(node[NameKey].ToString());
}