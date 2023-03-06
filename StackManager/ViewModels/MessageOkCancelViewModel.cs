using Prism.Services.Dialogs;
using StackManager.Extensions;

namespace StackManager.ViewModels
{
    class MessageOkCancelViewModel : DialogViewModel
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public MessageOkCancelViewModel()
        {
            Title = "系统提示";
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("Message");
        }
    }
}
