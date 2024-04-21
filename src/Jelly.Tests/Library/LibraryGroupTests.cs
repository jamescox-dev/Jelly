using Jelly.Library;

namespace Jelly.Tests.Library;

[TestFixture]
public class LibraryGroupTests
{
    [Test]
    public void EachOfTheMemberLibrariesAreLoadedIntoTheGivenScope()
    {
        var mockScope = new Mock<IScope>();
        var mockMemberLib1 = new Mock<ILibrary>();
        var mockMemberLib2 = new Mock<ILibrary>();
        var lib = new LibraryGroup();
        lib.AddLibrary(mockMemberLib1.Object);
        lib.AddLibrary(mockMemberLib2.Object);

        lib.LoadIntoScope(mockScope.Object);

        mockMemberLib1.Verify(m => m.LoadIntoScope(mockScope.Object));
        mockMemberLib2.Verify(m => m.LoadIntoScope(mockScope.Object));
    }
}