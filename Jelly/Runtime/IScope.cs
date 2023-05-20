namespace Jelly.Runtime;

public interface IScope
{
    IScope? OuterScope { get; }

    IEnumerable<string> GetVariableNames(bool localOnly = false);

    void DefineVariable(string name, Value initialValue);
    
    Value GetVariable(string name);

    void SetVariable(string name, Value value);

    IEnumerable<string> GetCommandNames(bool localOnly = false);

    void DefineCommand(string name, ICommand command);

    ICommand GetCommand(string name);

    void DefineHiddenValue(int id, Value initialValue);

    Value GetHiddenValue(int id);

    void SetHiddenValue(int id, Value value);
}