namespace Jelly.Parser;

public interface IParser
{
    DictValue? Parse(Scanner scanner);
}