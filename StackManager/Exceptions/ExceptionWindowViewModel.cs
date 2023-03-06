using System;

namespace StackManager.Exceptions
{
    class ExceptionWindowVM
    {
        public Exception Exception { get; }

        public string ExceptionType { get; }

        public ExceptionWindowVM(Exception exc)
        {
            Exception = exc;
            ExceptionType = exc.GetType().FullName;
        }
    }
}
