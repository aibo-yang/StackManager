using System.ComponentModel;
using System.Windows;

namespace Common.UI.WPF.Charts
{
    public class DataPoint : DependencyObject, INotifyPropertyChanged
    {
        public DataPoint()
        {
        }

        public DataPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public DataPoint(double x, double y, string label) : this(x, y)
        {
            Label = label;
        }

        public readonly static DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(OnChanged)));

        public object Content
        {
            get
            {
                return base.GetValue(ContentProperty);
            }
            internal set
            {
                base.SetValue(ContentProperty, value);
            }
        }

        public readonly static DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(DataPoint), new PropertyMetadata(null, new PropertyChangedCallback(OnChanged)));

        public string Label
        {
            get
            {
                return (string)base.GetValue(LabelProperty);
            }
            set
            {
                base.SetValue(LabelProperty, value);
            }
        }

        public readonly static DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(DataPoint), new PropertyMetadata(0, new PropertyChangedCallback(OnChanged)));

        public double X
        {
            get
            {
                return (double)base.GetValue(XProperty);
            }
            set
            {
                base.SetValue(XProperty, value);
            }
        }

        public readonly static DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(DataPoint), new PropertyMetadata(0, new PropertyChangedCallback(OnChanged)));
        
        public double Y
        {
            get
            {
                return (double)base.GetValue(YProperty);
            }
            set
            {
                base.SetValue(YProperty, value);
            }
        }


        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DataPoint).OnPropertyChanged(new PropertyChangedEventArgs(e.Property.Name));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
