using System.Reflection;

namespace Nubetico.Frontend.Helpers
{
    public static class NubeticoAssemblyProperties
    {
        public static string GetBuildDate()
        {
            Assembly curAssembly = typeof(Program).Assembly;
            return $"{curAssembly.GetCustomAttributes(false).OfType<AssemblyTitleAttribute>()?.FirstOrDefault()?.Title}";
        }
    }
}
