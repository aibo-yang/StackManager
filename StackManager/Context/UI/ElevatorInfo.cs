using System;
using Prism.Mvvm;

namespace StackManager.UI
{
    public class ElevatorInfo : BindableBase
    {
        private int index;
        public int Index
        {
            get { return index; }
            set { SetProperty(ref index, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private int currentCount;
        public int CurrentCount
        {
            get { return currentCount; }
            set { SetProperty(ref currentCount, value); }
        }
    }
}
