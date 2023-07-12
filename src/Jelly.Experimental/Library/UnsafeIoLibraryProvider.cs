namespace Jelly.Experimental.Library;

public class UnsafeIoLibraryProvider : IIoLibraryProvider
{
    public void Copy(string src, string dst)
    {
        if (Exists(src))
        {
            if (IsDir(src))
            {
                if (!Exists(dst))
                {
                    Directory.CreateDirectory(dst);
                }
                foreach (var item in ListDir(src))
                {
                    Copy(System.IO.Path.Join(src, item), System.IO.Path.Join(dst, item));
                }
            }
            else
            {
                try
                {
                    File.Copy(src, dst, true);
                }
                catch
                {
                    throw Error.Io($"Unable to copy '{src}' to '{dst}'.");
                }
            }
        }
        else
        {
            throw Error.Io($"Unable to copy '{src}' to '{dst}'.");
        }
    }

    public void Delete(string path, bool recursive)
    {
        try
        {
            Directory.Delete(path, recursive);
        }
        catch (Exception error)
        {
            if (error is DirectoryNotFoundException)
            {
                throw Error.Io($"'{path}' does not exist.");
            }
            try
            {
                File.Delete(path);
            }
            catch (FileNotFoundException)
            {
                throw Error.Io($"'{path}' does not exist.");
            }
            throw Error.Io($"Unable to delete '{path}'.");
        }
    }

    public bool Exists(string path)
    {
        return File.Exists(path) || Directory.Exists(path);
    }

    public string GetWorkingDir()
    {
        try
        {
            return Directory.GetCurrentDirectory();
        }
        catch
        {
            throw Error.Io("Could not get current working directory.");
        }
    }

    public bool IsDir(string path)
    {
        if (Directory.Exists(path))
        {
            return true;
        }
        else if (File.Exists(path))
        {
            return false;
        }
        throw Error.Io($"'{path}' does not exist.");
    }

    public bool IsReadOnly(string path)
    {
        try
        {
            return (File.GetAttributes(path) & FileAttributes.ReadOnly) != 0;
        }
        catch (FileNotFoundException)
        {
            throw Error.Io($"'{path}' does not exist.");
        }
        catch (DirectoryNotFoundException)
        {
            throw Error.Io($"'{path}' does not exist.");
        }
        catch (Exception)
        {
            throw Error.Io($"Could not get read-only status of '{path}'.");
        }
    }

    public IEnumerable<string> ListDir(string path)
    {
        try
        {
            return Directory.GetFileSystemEntries(path).Select(p => System.IO.Path.GetFileName(p));
        }
        catch (DirectoryNotFoundException)
        {
            throw Error.Io($"'{path}' does not exist.");
        }
        catch (IOException)
        {
            throw Error.Io($"'{path}' is a file.");
        }
        catch (Exception)
        {
            throw Error.Io("Could non list directory.");
        }
    }

    public void Move(string src, string dst)
    {
        try
        {
            Directory.Move(src, dst);
        }
        catch
        {
            try
            {
                File.Move(src, dst);
            }
            catch
            {
                throw Error.Io($"Unable to move '{src}' to '{dst}'.");
            }
        }
    }

    public string Path(IEnumerable<string> paths)
    {
        return System.IO.Path.Join(paths.ToArray());
    }

    public string ReadAll(string file)
    {
        try
        {
            return File.ReadAllText(file);
        }
        catch (FileNotFoundException)
        {
            throw Error.Io($"'{file}' does not exist.");
        }
        catch (DirectoryNotFoundException)
        {
            throw Error.Io($"'{file}' does not exist.");
        }
        catch (Exception)
        {
            throw Error.Io("Could not read file.");
        }
    }

    public void SetReadOnly(string path, bool readOnly)
    {
        try
        {
            var attrs = File.GetAttributes(path);
            if (readOnly)
            {
                attrs |= FileAttributes.ReadOnly;
            }
            else
            {
                attrs &= ~FileAttributes.ReadOnly;
            }
            File.SetAttributes(path, attrs);
        }
        catch (Exception error)
        {
            if (error is DirectoryNotFoundException || error is FileNotFoundException)
            {
                throw Error.Io($"'{path}' does not exist.");
            }
            throw Error.Io($"Could not set read-only status of '{path}'.");
        }
    }

    public void SetWorkingDir(string path)
    {
        try
        {
            Directory.SetCurrentDirectory(path);
        }
        catch (FileNotFoundException)
        {
            throw Error.Io($"'{path}' does not exist.");
        }
        catch (DirectoryNotFoundException)
        {
            throw Error.Io($"'{path}' does not exist.");
        }
        catch (Exception)
        {
            throw Error.Io("Could not set current working directory.");
        }
    }

    public void WriteAll(string file, string text)
    {
        try
        {
            File.WriteAllText(file, text);
        }
        catch (FileNotFoundException)
        {
            throw Error.Io($"'{file}' does not exist.");
        }
        catch (DirectoryNotFoundException)
        {
            throw Error.Io($"'{file}' does not exist.");
        }
        catch (Exception)
        {
            throw Error.Io("Could not write file.");
        }
    }
}