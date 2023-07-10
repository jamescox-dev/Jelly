namespace Jelly.Library;

public interface IIoLibraryProvider
{
    bool Exists(string path);

    string GetWorkingDir();

    void SetWorkingDir(string path);

    bool IsDir(string path);

    bool IsReadOnly(string path);

    void SetReadOnly(string path, bool readOnly);

    IEnumerable<string> ListDir(string path);

    string ReadAll(string file);

    void WriteAll(string file, string text);

    void Copy(string src, string dst);

    void Move(string src, string dst);

    void Delete(string path, bool recursive);

    string Path(IEnumerable<string> paths);
}