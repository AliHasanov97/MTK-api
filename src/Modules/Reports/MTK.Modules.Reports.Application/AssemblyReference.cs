using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MTK.Modules.Reports.Infrastructure")]

namespace MTK.Modules.Reports.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
