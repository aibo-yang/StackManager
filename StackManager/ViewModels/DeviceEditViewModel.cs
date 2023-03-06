using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using StackManager.Context.Domain;
using StackManager.Extensions;
using StackManager.Repositories;
using StackManager.UI;

namespace StackManager.ViewModels
{
    class DeviceEditViewModel : DialogViewModel
    {
        private readonly ILogger<DeviceEditViewModel> logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IUnitOfWork unitOfWork;

        readonly IRepository<DeviceCategory> deviceCategoryRepository;

        public DelegateCommand<string> ButtonCommands { get; set; }

        private DeviceCategoriesVM deviceCategoriesVM;
        public DeviceCategoriesVM DeviceCategoriesVM
        { 
            get { return deviceCategoriesVM; }
            set { SetProperty(ref deviceCategoriesVM, value); }
        }

        private DeviceCategoryVM deviceCategoryVM;
        public DeviceCategoryVM DeviceCategoryVM
        {
            get { return deviceCategoryVM; }
            set { SetProperty(ref deviceCategoryVM, value); }
        }

        public DeviceEditViewModel(ILogger<DeviceEditViewModel> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            Title = "配置设备信息";
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.deviceCategoryRepository = this.unitOfWork.GetRepository<DeviceCategory>();
            ButtonCommands = new DelegateCommand<string>(ButtonCommandsClick);

            RefershPageView();
        }

        void RefershPageView()
        {
            unitOfWork.ClearDbContext();
            DeviceCategoryVM = null;
            DeviceCategoriesVM = new DeviceCategoriesVM(this.deviceCategoryRepository.NoTrackingQuery().ToList());
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
                    if (DeviceCategoryVM == null)
                    {
                        PromptMessage.Message = "没有需要保存的数据";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(DeviceCategoryVM.Code))
                    {
                        PromptMessage.Message = "设备编码不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (deviceCategoryRepository.NoTrackingQuery().Any(x => x.Id != DeviceCategoryVM.Id && x.Code == DeviceCategoryVM.Code))
                    {
                        PromptMessage.Message = "设备编码重复，请检查后重试";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (DeviceCategoryVM.Id == Guid.Empty)
                    {
                        await deviceCategoryRepository.AddAsync(DeviceCategoryVM.DomainModel);
                    }
                    else
                    {
                        await deviceCategoryRepository.UpdateAsync(DeviceCategoryVM.DomainModel);
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
                    PromptMessage.Message = "保存设备配置信息失败，请重试";
                    PromptMessage.HasError = true;
                }
            }
        }
    }
}