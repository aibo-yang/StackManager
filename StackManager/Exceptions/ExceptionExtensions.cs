using System;
using System.Runtime.ExceptionServices;
using System.Windows;

namespace StackManager.Exceptions
{
    static class ExceptionExtensions
    {
        public static void ThrowOnDispatcher(this Exception ex)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                // preserve the callstack of the exception
                ExceptionDispatchInfo.Capture(ex).Throw();
            }));
        }
    }
}
