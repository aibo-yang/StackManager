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
    class ChangePasswordViewModel : DialogViewModel
    {
        private readonly ILogger<ChangePasswordViewModel> logger;
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

        private string newPassword;
        public string NewPassword
        {
            get { return newPassword; }
            set { SetProperty(ref newPassword, value); }
        }

        private string newPasswordConfirm;
        public string NewPasswordConfirm
        {
            get { return newPasswordConfirm; }
            set { SetProperty(ref newPasswordConfirm, value); }
        }

        public ChangePasswordViewModel(ILogger<ChangePasswordViewModel> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            Title = "密码更改页面";
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.settingRepository = this.unitOfWork.GetRepository<Setting>();
            SettingVM = new SettingVM(this.settingRepository.TrackingQuery().SingleOrDefault());
            ButtonCommands = new DelegateCommand<string>(ButtonCommandsClick);
        }

        private async void ButtonCommandsClick(string commandName)
        {
            PromptMessage.HasError = null;

            if (commandName == "Cancel")
            {
                RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
            }
            else if (commandName == "Enter")
            {
                if(string.IsNullOrEmpty(Password))
                {
                    PromptMessage.Message = "旧密码不能为空";
                    PromptMessage.HasError = true;
                    return;
                }

                if (string.IsNullOrEmpty(NewPassword))
                {
                    PromptMessage.Message = "新密码不能为空";
                    PromptMessage.HasError = true;
                    return;
                }

                if (NewPassword != NewPasswordConfirm)
                {
                    PromptMessage.Message = "两次新密码不相同";
                    PromptMessage.HasError = true;
                    return;
                }

                var securePwd = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Password + "KSTOPA")).Select(x => x.ToString("X2")));
                if (securePwd != settingVM.Password)
                {
                    PromptMessage.Message = "旧密码错误, 请重试";
                    PromptMessage.HasError = true;
                    return;
                }
                else
                {
                    settingVM.Password = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(NewPassword + "KSTOPA")).Select(x => x.ToString("X2")));
                    if (await unitOfWork.SaveChangesAsync(async entry =>
                    {
                        entry.Reload();
                        return await Task.FromResult(false);
                    }))
                    {
                        PromptMessage.Message = "更改密码成功";
                        PromptMessage.HasError = null;
                    }
                    else
                    {
                        PromptMessage.Message = "保存数据失败，请重试";
                        PromptMessage.HasError = true;
                    }
                    // RaiseRequestClose(new DialogResult(ButtonResult.OK));
                }
            }
        }
    }
}
