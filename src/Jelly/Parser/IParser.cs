namespace Jelly.Parser;

public interface IParser
{
    DictionaryValue? Parse(Scanner scanner);
}