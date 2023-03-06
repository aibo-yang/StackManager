using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using System;
using Prism.Mvvm;
using StackManager.Context.Domain;
using StackManager.Extensions;
using StackManager.Repositories;
using StackManager.UI;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Security.Cryptography;
using System.Text;
using Prism.Services.Dialogs;

namespace StackManager.ViewModels
{
    class LoginViewModel : DialogViewModel
    {
        private readonly ILogger<LoginViewModel> logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IUnitOfWork unitOfWork;

        readonly IRepository<Setting> settingRepository;

        public DelegateCommand<string> ButtonCommands { get; set; }
        public DelegateCommand<PasswordBox> PasswordChangedCommand { get; set; }

        private SettingVM settingVM;
        public SettingVM SettingVM
        {
            get { return settingVM; }
            set { SetProperty(ref settingVM, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        public LoginViewModel(ILogger<LoginViewModel> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            Title = "系统登录页面";
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.settingRepository = this.unitOfWork.GetRepository<Setting>();
            SettingVM = new SettingVM(this.settingRepository.NoTrackingQuery().SingleOrDefault());
            ButtonCommands = new DelegateCommand<string>(ButtonCommandsClick);
        }

        private void ButtonCommandsClick(string commandName)
        {
            PromptMessage.HasError = null;

            if (commandName == "Cancel")
            {
                RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
            }
            else if (commandName == "Login")
            {
                if(string.IsNullOrEmpty(Password))
                {
                    PromptMessage.Message = "密码不能为空";
                    PromptMessage.HasError = true;
                    return;
                }

                var securePwd = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Password + "KSTOPA")).Select(x => x.ToString("X2")));
                if (securePwd != settingVM.Password)
                {
                    PromptMessage.Message = "密码错误, 请重试";
                    PromptMessage.HasError = true;
                    return;
                }
                else
                {
                    RaiseRequestClose(new DialogResult(ButtonResult.OK));
                }
            }
        }
    }
}
