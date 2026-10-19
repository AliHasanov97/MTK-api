using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MTK.Modules.Communications.Infrastructure")]
[assembly: InternalsVisibleTo("MTK.Tests")]

namespace MTK.Modules.Communications.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
