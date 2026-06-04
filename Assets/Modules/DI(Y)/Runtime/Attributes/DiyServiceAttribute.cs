using System;
using JetBrains.Annotations;

namespace DependentlyInjectYourself.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DiyServiceAttribute : Attribute
    {
        [CanBeNull] public Type ServiceType { get; }

        public DiyServiceAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public DiyServiceAttribute()
        {
        }
    }
}