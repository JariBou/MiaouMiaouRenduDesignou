using System;

namespace DependentlyInjectYourself.API
{
    public interface IDiyService<TService> /*: IDisposable*/ where TService : class
    {
        // public event Action ServiceDestroyed;
    }
}