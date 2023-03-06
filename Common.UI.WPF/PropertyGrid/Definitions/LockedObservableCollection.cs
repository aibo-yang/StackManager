using System;
using System.Collections.ObjectModel;

namespace Common.UI.WPF.PropertyGrid
{
    public abstract class LockedObservableCollection<T> : ObservableCollection<T> where T : LockedDependencyObject
    {
        internal LockedObservableCollection()
        {
        }

        protected override void InsertItem(int index, T item)
        {
            if (item == null)
            {
                throw new InvalidOperationException("Cannot insert null items in the collection.");
            }

            item.Lock();
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (item == null)
            {
                throw new InvalidOperationException(@"Cannot insert null items in the collection.");
            }

            item.Lock();
            base.SetItem(index, item);
        }
    }
}
