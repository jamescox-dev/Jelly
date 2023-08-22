namespace Jelly.Library;

public class LibraryGroup: ILibrary
{
    public readonly List<ILibrary> _memberLibraries = new();

    public void AddLibrary(ILibrary memberLibrary)
    {
        _memberLibraries.Add(memberLibrary);
    }

    public void LoadIntoScope(IScope scope)
    {
        foreach (var memberLibrary in _memberLibraries)
        {
            memberLibrary.LoadIntoScope(scope);
        }
    }
}