namespace Jelly.Shell;

public interface IHistoryManager
{
    void AddHistory(string command);

    void LoadHistory();

    void SaveHistory();
}