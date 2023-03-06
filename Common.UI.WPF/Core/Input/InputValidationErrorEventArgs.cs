using System;

namespace Common.UI.WPF.Core.Input
{
    public delegate void InputValidationErrorEventHandler(object sender, InputValidationErrorEventArgs e);

    public class InputValidationErrorEventArgs : EventArgs
    {
        public InputValidationErrorEventArgs(Exception e)
        {
            Exception = e;
        }

        private Exception exception;
        public Exception Exception
        {
            get
            {
                return exception;
            }
            private set
            {
                exception = value;
            }
        }

        private bool throwException;
        public bool ThrowException
        {
            get
            {
                return throwException;
            }
            set
            {
                throwException = value;
            }
        }
    }
}
