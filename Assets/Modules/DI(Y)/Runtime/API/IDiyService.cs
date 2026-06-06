using System;
using JetBrains.Annotations;

namespace DependentlyInjectYourself.API
{
    public interface IDiyService
    {
        public event Action ServiceDestroyed;
    }
    
    public interface IDiyService<[UsedImplicitly] TService> : IDiyService/*, IDisposable*/ where TService : class
    {
        // public Type ServiceType => typeof(TService); // I don't particularly enjoy this either but I am willing to make the exception to avoid using too much reflection for performance reasons
    }
}