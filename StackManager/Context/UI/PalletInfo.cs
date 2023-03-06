using System.Windows.Media;
using Prism.Mvvm;

namespace StackManager.UI
{
    public class PalletInfo : BindableBase
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

        private string type;
        public string Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        private int currentCount;
        public int CurrentCount
        {
            get { return currentCount; }
            set 
            {
                SetProperty(ref currentCount, value);
                Percentage = CurrentCount / (MaxCount*1.0);
            }
        }

        private int maxCount;
        public int MaxCount
        {
            get { return maxCount; }
            set 
            {
                SetProperty(ref maxCount, value);
                Percentage = CurrentCount / (MaxCount * 1.0);
            }
        }

        private double percentage;
        public double Percentage
        {
            get { return percentage; }
            set { SetProperty(ref percentage, value); }
        }

        private bool isExist;
        public bool IsExist
        {
            get { return isExist; }
            set { SetProperty(ref isExist, value); }
        }

        private Brush palletBrush;
        public Brush PalletBrush
        {
            get { return palletBrush; }
            set { SetProperty(ref palletBrush, value); }
        }

        private double canvasLeft;
        public double CanvasLeft
        {
            get { return canvasLeft; }
            set { SetProperty(ref canvasLeft, value); }
        }

        private double canvasTop;
        public double CanvasTop
        {
            get { return canvasTop; }
            set { SetProperty(ref canvasTop, value); }
        }

        private int gridRow;
        public int GridRow
        {
            get { return gridRow; }
            set { gridRow = value; RaisePropertyChanged(); }
        }

        private int gridColumn;
        public int GridColumn
        {
            get { return gridColumn; }
            set { gridColumn = value; RaisePropertyChanged(); }
        }
    }
}
