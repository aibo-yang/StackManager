using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.UI.WPF.Charts
{
    public class ListBase<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        private readonly List<T> interList;

        public int Count
        {
            get { return GetCount(); }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<T>)interList).IsReadOnly; }
        }

        public virtual T this[int index] 
        {
            get
            {
                return interList[index];
            }
            set
            {
                if (!interList[index].Equals(value))
                {
                    var item = new T[] { interList[index]};
                    var array = new T[] { value };
                    var changedEventArg = new ListChangedEventArgs<T>(index, item, array, ListActionType.Set);
                    OnBeforeListChanged(changedEventArg);
                    interList[index] = value;
                    OnListChanged(changedEventArg);
                }
            }
        }

        object IList.this[int index] 
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (T)value;
            }
        }

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public bool IsFixedSize => false;

        public event ListChangedEventHandler<T> BeforeListChanged;
        public event ListChangedEventHandler<T> ListChanged;

        public ListBase()
        {
            interList = new List<T>();
        }

        public ListBase(int capacity)
        {
            interList = new List<T>(capacity);
        }

        public ListBase(IEnumerable<T> collection)
        {
            interList = new List<T>(collection);
        }

        public virtual void Add(T item)
        {
            int count = interList.Count;
            T[] array = new T[] { item };
            var changedEventArg = new ListChangedEventArgs<T>(count, null, array, ListActionType.Insert);
            OnBeforeListChanged(changedEventArg);
            interList.Add(item);
            OnListChanged(changedEventArg);
        }

        public virtual void AddRange(T[] items)
        {
            var changedEventArg = new ListChangedEventArgs<T>(interList.Count, null, items, ListActionType.Insert);
            OnBeforeListChanged(changedEventArg);
            interList.AddRange(items);
            OnListChanged(changedEventArg);
        }

        public virtual void Clear()
        {
            T[] array = interList.ToArray();
            var listChangedEventArg = new ListChangedEventArgs<T>(-1, array, null, ListActionType.Clear);
            OnBeforeListChanged(listChangedEventArg);
            interList.Clear();
            OnListChanged(listChangedEventArg);
        }

        public virtual bool Contains(T item)
        {
            return interList.Contains(item);
        }

        public virtual void CopyTo(T[] array, int index)
        {
            interList.CopyTo(array, index);
        }

        protected virtual int GetCount() => interList.Count;

        public virtual IEnumerator<T> GetEnumerator()
        {
            return interList.GetEnumerator();
        }

        public virtual int IndexOf(T item)
        {
            return interList.IndexOf(item);
        }

        public virtual void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public virtual void Insert(int index, T item)
        {
            if (index < 0 || index > interList.Count)
            {
                throw new ArgumentNullException("index");
            }

            var array = new T[] { item };
            var changedEventArg = new ListChangedEventArgs<T>(index, null, array, ListActionType.Insert);
            OnBeforeListChanged(changedEventArg);
            interList.Insert(index, item);
            OnListChanged(changedEventArg);
        }

        public virtual bool Remove(T item)
        {
            int num = IndexOf(item);
            var array = new T[] { item };
            var changedEventArg = new ListChangedEventArgs<T>(num, array, null, ListActionType.Remove);
            this.OnBeforeListChanged(changedEventArg);
            var flag = interList.Remove(item);
            if (flag)
            {
                OnListChanged(changedEventArg);
            }
            return flag;
        }

        public virtual void RemoveAt(int index)
        {
            var items = new T[] { this[index] };
            var changedEventArg = new ListChangedEventArgs<T>(index, items, null, ListActionType.Remove);
            OnBeforeListChanged(changedEventArg);
            interList.RemoveAt(index);
            OnListChanged(changedEventArg);
        }

        protected void OnBeforeListChanged(ListChangedEventArgs<T> args)
        {
            RaiseBeforeListChanged(this, args);
        }

        protected void OnListChanged(ListChangedEventArgs<T> args)
        {
            RaiseListChanged(this, args);
        }

        private void RaiseBeforeListChanged(object sender, ListChangedEventArgs<T> args)
        {
            BeforeListChanged?.Invoke(sender, args);
        }

        private void RaiseListChanged(object sender, ListChangedEventArgs<T> args)
        {
            ListChanged?.Invoke(sender, args);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)interList).GetEnumerator();
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return IndexOf((T)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        public virtual T[] ToArray()
        {
            return interList.ToArray();
        }
    }
}
