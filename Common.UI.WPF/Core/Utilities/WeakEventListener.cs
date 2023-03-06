using System;
using System.Windows;

namespace Common.UI.WPF.Core.Utilities
{
    internal class WeakEventListener<TArgs> : IWeakEventListener where TArgs : EventArgs
    {
        private Action<object, TArgs> callback;

        public WeakEventListener(Action<object, TArgs> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            this.callback = callback;
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            callback(sender, (TArgs)e);
            return true;
        }
    }
}
