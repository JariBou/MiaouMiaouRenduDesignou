using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependentlyInjectYourself.Utils
{
    public static class AssemblyCacheUtils
    {
        private static List<Assembly> _assemblies;
        private static List<Assembly> _nonUnityAssemblies;
        public static List<Assembly> Assemblies => _assemblies ??= AppDomain.CurrentDomain.GetAssemblies().ToList();

        public static List<Assembly> NonUnityAssemblies => _nonUnityAssemblies ??= AppDomain.CurrentDomain.GetAssemblies()
                                                                                            .Where(assembly => !assembly.GetName().ToString()
                                                                                                       .StartsWith("Unity")).ToList();
    }
}