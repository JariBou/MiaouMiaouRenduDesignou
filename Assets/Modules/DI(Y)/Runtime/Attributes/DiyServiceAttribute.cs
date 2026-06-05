using System;
using JetBrains.Annotations;

namespace DependentlyInjectYourself.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DiyServiceAttribute : Attribute
    {
        [CanBeNull] public Type ServiceType { get; }
        public ServiceLifetime ServiceLifetime { get; }
        public ServiceScope ServiceScope { get; }

        public DiyServiceAttribute(Type serviceType, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, ServiceScope serviceScope = ServiceScope.Project)
        {
            ServiceType = serviceType;
            ServiceLifetime = serviceLifetime;
            ServiceScope = serviceScope;
        }

        public DiyServiceAttribute(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, ServiceScope serviceScope = ServiceScope.Project)
        {
            ServiceLifetime = serviceLifetime;
            ServiceScope = serviceScope;
        }
    }
}