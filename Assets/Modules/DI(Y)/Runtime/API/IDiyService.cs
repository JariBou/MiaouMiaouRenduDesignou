using System;

namespace DependentlyInjectYourself.API
{
    public interface IDiyService // Useless for now
    {
        public event Action ServiceDestroyed;
    }
}