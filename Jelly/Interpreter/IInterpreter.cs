namespace Jelly.Interpreter;

public interface IInterpreter
{
    Value Evaluate(DictionaryValue node, IInterpreter interpreter);
}