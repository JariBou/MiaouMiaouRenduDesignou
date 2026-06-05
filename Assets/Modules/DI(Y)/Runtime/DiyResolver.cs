using System;
using System.Linq;
using System.Reflection;
using DependentlyInjectYourself.Attributes;

namespace DependentlyInjectYourself
{
    public class DiyResolver
    {
        public static void ResolveType(Type type)
        {
            MethodInfo method = GetDiyInitializerMethodInfoOfType(type);
            
        }
        
        public static MethodInfo GetDiyInitializerMethodInfoOfType(Type type)
        {
            MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return methodInfos
                       .FirstOrDefault(info => info.GetCustomAttributes(typeof(DiyInitializerMethodAttribute), false).Any());
        }
    }
}