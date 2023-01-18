namespace Jelly;

using Jelly.Commands;
using Jelly.Values;

public interface IScope
{
    void DefineCommand(string name, ICommand command);

    void DefineVariable(string name, StringValue initialValue);
    
    ICommand GetCommand(string name);
    
    Value GetVariable(string name);
}