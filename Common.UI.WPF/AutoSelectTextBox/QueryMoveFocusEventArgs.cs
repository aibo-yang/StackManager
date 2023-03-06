using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace Common.UI.WPF
{
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void QueryMoveFocusEventHandler(object sender, QueryMoveFocusEventArgs e);

    public class QueryMoveFocusEventArgs : RoutedEventArgs
    {
        //default CTOR private to prevent its usage.
        private QueryMoveFocusEventArgs()
        {
        }

        //internal to prevent anybody from building this type of event.
        internal QueryMoveFocusEventArgs(FocusNavigationDirection direction, bool reachedMaxLength)
            : base(AutoSelectTextBox.QueryMoveFocusEvent)
        {
            this.navigationDirection = direction;
            this.reachedMaxLength = reachedMaxLength;
        }

        public FocusNavigationDirection FocusNavigationDirection
        {
            get
            {
                return navigationDirection;
            }
        }

        public bool ReachedMaxLength
        {
            get
            {
                return reachedMaxLength;
            }
        }

        public bool CanMoveFocus
        {
            get
            {
                return canMove;
            }
            set
            {
                canMove = value;
            }
        }

        private FocusNavigationDirection navigationDirection;
        private bool reachedMaxLength;
        private bool canMove = true; //defaults to true... if nobody does nothing, then its capable of moving focus.
    }
}
