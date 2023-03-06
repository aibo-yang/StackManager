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

namespace StackManager.ViewModels
{
    class ProfileEditViewModel : DialogViewModel
    {
        private readonly ILogger<ProfileEditViewModel> logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IUnitOfWork unitOfWork;

        readonly IRepository<Setting> settingRepository;

        public DelegateCommand<string> ButtonCommands { get; set; }

        private SettingVM settingVM;
        public SettingVM SettingVM
        {
            get { return settingVM; }
            set { SetProperty(ref settingVM, value); }
        }

        public ProfileEditViewModel(ILogger<ProfileEditViewModel> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            Title = "配置基础数据";
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.settingRepository = this.unitOfWork.GetRepository<Setting>();

            ButtonCommands = new DelegateCommand<string>(ButtonCommandsClick);

            RefershPageView();
        }

        void RefershPageView()
        {
            unitOfWork.ClearDbContext();

            SettingVM = null;
            SettingVM = new SettingVM(this.settingRepository
                .NoTrackingQuery()
                .SingleOrDefault());
        }

        private async void ButtonCommandsClick(string commandName)
        {
            PromptMessage.HasError = null;

            if (commandName == "PageRefersh")
            {
                RefershPageView();
            }
            else if (commandName == "PageUp")
            {
            }
            else if (commandName == "PageDown")
            {
            }
            else if (commandName == "DataNew")
            {
            }
            else
            {
                if (commandName == "DataDelete")
                {
                }
                else if (commandName == "DataSave")
                {
                    if (SettingVM == null)
                    {
                        PromptMessage.Message = "没有需要保存的数据";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(SettingVM.Name))
                    {
                        PromptMessage.Message = "设备名称不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(SettingVM.SerialNumber))
                    {
                        PromptMessage.Message = "设备编号不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(SettingVM.MesUri))
                    {
                        PromptMessage.Message = "MES地址不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(SettingVM.MesSecret))
                    {
                        PromptMessage.Message = "MES密码不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(SettingVM.MesTokenID))
                    {
                        PromptMessage.Message = "MES令牌不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(SettingVM.PqmUri))
                    {
                        PromptMessage.Message = "PQM地址不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (SettingVM.Id == Guid.Empty)
                    {
                        await settingRepository.AddAsync(SettingVM.DomainModel);
                    }
                    else
                    {
                        await settingRepository.UpdateAsync(SettingVM.DomainModel);
                    }
                }

                if (await unitOfWork.SaveChangesAsync(async entry =>
                {
                    entry.Reload();
                    return await Task.FromResult(false);
                }))
                {
                    PromptMessage.Message = "操作成功";
                    PromptMessage.HasError = false;
                    RefershPageView();
                }
                else
                {
                    PromptMessage.Message = "保存基础数据失败，请重试";
                    PromptMessage.HasError = true;
                }
            }
        }
    }
}
