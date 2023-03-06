using System;

namespace StackManager.Exceptions
{
    public class UnknownAppDomainException : Exception
    {
        public UnknownAppDomainException(string msg) : base(msg)
        {
        }
    }
}
