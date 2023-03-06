using System.ComponentModel;

namespace StackManager.Context.UI
{
    abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new(propertyName);
                PropertyChanged(this, e);
            }
        }
    }
}
