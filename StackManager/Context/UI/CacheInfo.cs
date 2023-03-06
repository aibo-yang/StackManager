using System.Windows.Media;
using Prism.Mvvm;

namespace StackManager.UI
{
    public class CacheInfo : BindableBase
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
            set { name = value; RaisePropertyChanged(); }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { count = value; RaisePropertyChanged(); }
        }
    }
}
