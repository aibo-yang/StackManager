using System.Windows;
using Microsoft.Xaml.Behaviors;
using Prism.Services.Dialogs;
using StackManager.Behaviors;

namespace StackManager.Views
{
    public partial class MainView : Window
    {
        private readonly IDialogService dialogService;

        public MainView(IDialogService dialogService)
        {
            InitializeComponent();
            this.dialogService = dialogService;
            // Interaction.GetBehaviors(orderList).Add(new ListBoxAutoScrollBehavior());
        }

        private void Button_SystemExit_Click(object sender, RoutedEventArgs e)
        {
            dialogService.ShowDialog("MessageOkCancelView", new DialogParameters($"Message=确定关闭软件并退出系统吗？"), r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    Application.Current.MainWindow.Close();
                }
            });
        }
    }
}
