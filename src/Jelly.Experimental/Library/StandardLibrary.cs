namespace Jelly.Experimental.Library;

public class StandardLibrary : LibraryGroup
{
    public StandardLibrary()
    {
        AddLibrary(new Jelly.Library.CollectionsLibrary());
        AddLibrary(new CollectionsLibrary());
        AddLibrary(new Jelly.Library.CoreLibrary());
        AddLibrary(new CoreLibrary());
        AddLibrary(new Jelly.Library.MathLibrary());
        AddLibrary(new MathLibrary());
        AddLibrary(new RandomLibrary());
        AddLibrary(new StringLibrary());
        AddLibrary(new UtilsLibrary());
    }
}