using System.Windows;

namespace StackManager.Exceptions
{
    public partial class ExceptionWindow : Window
    {
        public ExceptionWindow()
        {
            InitializeComponent();
        }

        private void OnExitAppClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void OnExceptionWindowClosed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
