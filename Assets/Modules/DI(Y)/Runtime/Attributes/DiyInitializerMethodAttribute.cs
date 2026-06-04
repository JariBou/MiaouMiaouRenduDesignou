using System;

namespace DependentlyInjectYourself.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DiyInitializerMethodAttribute : Attribute
    {
    }
}