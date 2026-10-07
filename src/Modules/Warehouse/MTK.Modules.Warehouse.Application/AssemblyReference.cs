using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MTK.Modules.Warehouse.Infrastructure")]

namespace MTK.Modules.Warehouse.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
