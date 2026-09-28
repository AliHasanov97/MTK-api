using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MTK.Modules.Payments.Infrastructure")]

namespace MTK.Modules.Payments.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
