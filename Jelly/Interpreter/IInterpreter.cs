namespace Jelly.Interpreter;

public interface IInterpreter
{
    Value Evaluate(Scope scope, DictionaryValue node, IInterpreter interpreter);
}