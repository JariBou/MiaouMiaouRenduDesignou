using System;

namespace DependentlyInjectYourself.Exceptions
{
    public sealed class MissingServiceException : Exception
    {
        public MissingServiceException(string message) : base(message)
        {
        }
    }
}