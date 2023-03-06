using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace StackManager.Extensions
{
    class PromptMessage : BindableBase
    {
        private bool? hasError = false;
        public bool? HasError
        {
            get { return hasError; }
            set { SetProperty(ref hasError, value); }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public PromptMessage()
        {
        }
    }

    class DialogViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        private DelegateCommand<ButtonResult?> dialogClosedCommand;
        public DelegateCommand<ButtonResult?> DialogClosedCommand => 
            dialogClosedCommand ?? (dialogClosedCommand = new DelegateCommand<ButtonResult?>(CloseDialog));

        public PromptMessage PromptMessage { get; }

        public DialogViewModel()
        {
            PromptMessage = new PromptMessage();
        }

        private string iconSource;
        public string IconSource
        {
            get { return iconSource; }
            set { SetProperty(ref iconSource, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        protected virtual void CloseDialog(ButtonResult? result)
        {
            if (result == null)
            {
                return;
            }
            RaiseRequestClose(new DialogResult((ButtonResult)result));
        }
    }
}
