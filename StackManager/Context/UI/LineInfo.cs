using System.Windows.Media;
using Prism.Mvvm;

namespace StackManager.UI
{
    class LineInfo : BindableBase
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

        //private string type;
        //public string Type
        //{
        //    get { return type; }
        //    set { type = value; RaisePropertyChanged(); }
        //}

        private int currentCount;
        public int CurrentCount
        {
            get { return currentCount; }
            set { currentCount = value; RaisePropertyChanged(); }
        }

        private int maxCount;
        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; RaisePropertyChanged(); }
        }

        //private int cycleTime;
        //public int CycleTime
        //{
        //    get { return cycleTime; }
        //    set { cycleTime = value; RaisePropertyChanged(); }
        //}

        private Brush lineBrush;
        public Brush LineBrush
        {
            get { return lineBrush; }
            set { lineBrush = value; RaisePropertyChanged(); }
        }

        private double canvasLeft;
        public double CanvasLeft
        {
            get { return canvasLeft; }
            set { canvasLeft = value; RaisePropertyChanged(); }
        }
    }
}
