namespace Jelly;

using Jelly.Commands;
using Jelly.Values;

public interface IScope
{
    Scope? OuterScope { get; }

    void DefineVariable(string name, Value initialValue);
    
    Value GetVariable(string name);

    void SetVariable(string name, Value value);

    IEnumerable<string> GetCommands(bool localOnly = false);

    void DefineCommand(string name, ICommand command);

    ICommand GetCommand(string name);

    void DefineHiddenValue(int id, Value initialValue);

    Value GetHiddenValue(int id);

    void SetHiddenValue(int id, Value value);
}