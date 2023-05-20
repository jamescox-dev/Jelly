namespace Jelly.Commands.ArgParsers;

public class SingleArgParser : IArgParser
{
    readonly Value _name;

    public SingleArgParser(string name)
    {
        _name = name.ToValue();
    }

    public DictionaryValue Parse(ListValue args, int index=0)
    {
        if (args.Count > index)
        {
            return new DictionaryValue(_name, args[index]);
        }
        throw Error.Arg($"Expected '{_name}' argument.");
    }
}