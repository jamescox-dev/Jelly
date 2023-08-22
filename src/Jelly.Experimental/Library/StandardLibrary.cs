namespace Jelly.Experimental.Library;

public class StandardLibrary : LibraryGroup
{
    public StandardLibrary()
    {
        AddLibrary(new CollectionsLibrary());
        AddLibrary(new CoreLibrary());
        AddLibrary(new MathLibrary());
        AddLibrary(new RandomLibrary());
        AddLibrary(new StringLibrary());
        AddLibrary(new UtilsLibrary());
    }
}