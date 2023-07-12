namespace Jelly.Shell;

using System.Text.Json;

public class ReadLineHistoryManager : IHistoryManager
{
    readonly string _settingsFile;

    public ReadLineHistoryManager(string settingsFile)
    {
        _settingsFile = settingsFile;
    }

    public void AddHistory(string command)
    {
        ReadLine.AddHistory(command);
    }

    public void LoadHistory()
    {
        if (File.Exists("history.json"))
        {
            using var history = new FileStream(_settingsFile, FileMode.Open);
            JsonSerializer.Deserialize<List<string>>(history);
        }
    }

    public void SaveHistory()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsFile)!);

        using var history = new FileStream(_settingsFile, FileMode.OpenOrCreate, FileAccess.Write);
        JsonSerializer.Serialize(history, ReadLine.GetHistory());
        history.Close();
    }
}