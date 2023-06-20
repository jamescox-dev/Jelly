using System.Reflection;
using System.Runtime.CompilerServices;

[assembly : InternalsVisibleTo("Jelly.Experimental")]
[assembly : InternalsVisibleTo("Jelly.Tests")]

namespace Jelly
{
    public static class JellyInfo
    {
        public const int Major = 0;
        public const int Minor = 1;
        public const int Revision = 0;
        public const string Tag = "prealpha";

        public static string VersionString =>
            $"{Major}.{Minor}.{Revision}" + (!string.IsNullOrEmpty(Tag) ? "-" + Tag : string.Empty);
    }
}