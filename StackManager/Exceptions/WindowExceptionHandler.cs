using System;
using System.Windows;

namespace StackManager.Exceptions
{
    class WindowExceptionHandler : GlobalExceptionHandlerBase
    {
        /// <summary>
        /// This method opens a new ExceptionWindow with the
        /// passed exception object as datacontext.
        /// </summary>
        public override void OnUnhandledException(Exception e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                var exceptionWindow = new ExceptionWindow
                {
                    DataContext = new ExceptionWindowVM(e)
                };
                exceptionWindow.Show();
            }));
        }
    }
}
