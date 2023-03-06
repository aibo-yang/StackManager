using System;

namespace Common.UI.WPF.Charts
{
    public class ListChangedEventArgs<T> : EventArgs
    {
        private ListActionType actionType;
        public ListActionType ActionType
        {
            get
            {
                return actionType;
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
        }

        private T[] oldItems;
        public T[] OldItems
        {
            get
            {
                return oldItems;
            }
        }

        private T[] newItems;
        public T[] NewItems
        {
            get
            {
                return newItems;
            }
        }

        public ListChangedEventArgs(int index, T[] oldItems, T[] newItems, ListActionType actionType)
        {
            this.index = index;
            this.oldItems = oldItems;
            this.newItems = newItems;
            this.actionType = actionType;
        }
    }
}
