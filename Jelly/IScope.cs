namespace Jelly;

using Jelly.Commands;
using Jelly.Values;

public interface IScope
{
    void DefineVariable(string name, Value initialValue);
    
    Value GetVariable(string name);

    void SetVariable(string name, Value value);

    void DefineCommand(string name, ICommand command);

    ICommand GetCommand(string name);
}