using System.Reflection;
using System.Runtime.CompilerServices;

[assembly : InternalsVisibleTo("Jelly.Experimental")]
[assembly : InternalsVisibleTo("Jelly.Tests")]
[assembly : InternalsVisibleTo("Jelly.Shell.Tests")]

namespace Jelly
{
    public static class JellyInfo
    {
        public const int Major = 0;
        public const int Minor = 0;
        public const int Revision = 1;
        public const string Tag = "dev4";

        public static string VersionString =>
            $"{Major}.{Minor}.{Revision}" + (!string.IsNullOrEmpty(Tag) ? "-" + Tag : string.Empty);
    }
}