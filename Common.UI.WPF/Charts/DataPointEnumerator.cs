using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.UI.WPF.Charts
{
    public class DataPointEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator where T : DataPoint
    {
        private int index;
        private DataPointsList<T> dataList;

        private T current;
        public T Current
        {
            get
            {
                return current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public DataPointEnumerator(DataPointsList<T> list)
        {
            dataList = list;
            index = 0;
            current = default(T);
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            bool flag = false;
            if (index >= dataList.Count)
            {
                index = dataList.Count + 1;
                current = default(T);
            }
            else
            {
                current = dataList[index];
                index++;
                flag = true;
            }
            return flag;
        }

        public void Reset()
        {
            index = 0;
            current = default(T);
        }
    }
}
