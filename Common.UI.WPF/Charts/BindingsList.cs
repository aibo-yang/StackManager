using System;
using System.Collections;

#pragma warning disable 649

namespace Common.UI.WPF.Charts
{
    public class BindingsList<T> : ListBase<T> where T : BindingInfo
    {
        private readonly Hashtable hashTable;

        public BindingsList()
        {
        }

        public override void Add(T item)
        {
            if (hashTable[item.PropertyName] != null)
            {
                throw new ArgumentException("Binding for such Property has been already added.", item.PropertyName.ToString());
            }

            hashTable[item.PropertyName] = item;
            base.Add(item);
        }

        public override void AddRange(T[] items)
        {
            var array = items;
            for (int i = 0; i < array.Length; i++)
            {
                InsertItem(array[i]);
            }
            base.AddRange(items);
        }

        public override void Clear()
        {
            foreach (T t in this)
            {
                RemoveItem(t);
            }
            base.Clear();
        }

        public override void Insert(int index, T item)
        {
            base.Insert(index, item);
        }

        private void InsertItem(BindingInfo item)
        {
            if (hashTable[item.PropertyName] != null)
            {
                throw new ArgumentException("Binding for such Property has been already added.", item.PropertyName.ToString());
            }
            hashTable[item.PropertyName] = item;
        }

        public override bool Remove(T item)
        {
            RemoveItem(item);
            return base.Remove(item);
        }

        public override void RemoveAt(int index)
        {
            if (index < base.Count && index >= 0)
            {
                RemoveItem(this[index]);
            }
            base.RemoveAt(index);
        }

        private void RemoveItem(BindingInfo item)
        {
            hashTable.Remove(item.PropertyName);
        }
    }
}
