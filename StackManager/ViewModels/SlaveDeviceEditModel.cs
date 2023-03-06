using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using StackManager.Context.Domain;
using StackManager.Context.UI;
using StackManager.Extensions;
using StackManager.Repositories;
using StackManager.UI;

namespace StackManager.ViewModels
{
    class SlaveDeviceEditModel : DialogViewModel
    {
        private readonly ILogger<SlaveDeviceEditModel> logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IUnitOfWork unitOfWork;

        readonly IRepository<SlaveDevice> slavedeviceRepository;

        public DelegateCommand<string> ButtonCommands { get; set; }

        private SlaveDevicesVM slavedvicesVM;
        public SlaveDevicesVM SlaveDevicesVM
        {
            get { return slavedvicesVM; }
            set { SetProperty(ref slavedvicesVM, value); }
        }

        private SlaveDeviceVM slavedeviceVM;
        public SlaveDeviceVM SlaveDeviceVM
        {
            get { return slavedeviceVM; }
            set { SetProperty(ref slavedeviceVM, value); }
        }

        public SlaveDeviceEditModel(ILogger<SlaveDeviceEditModel> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            Title = "配置设备信息";
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.slavedeviceRepository = this.unitOfWork.GetRepository<SlaveDevice>();
            ButtonCommands = new DelegateCommand<string>(ButtonCommandsClick);

            RefershPageView();
        }

        void RefershPageView()
        {
            unitOfWork.ClearDbContext();
            SlaveDeviceVM = null;
            SlaveDevicesVM = new SlaveDevicesVM(this.slavedeviceRepository.NoTrackingQuery().ToList());
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
                if (SlaveDevicesVM.Any(x => x.Id == Guid.Empty))
                {
                    PromptMessage.Message = "请先保存修改后的数据再执行新增操作";
                    PromptMessage.HasError = true;
                    return;
                }
                SlaveDeviceVM = new SlaveDeviceVM();
                SlaveDevicesVM.Add(SlaveDeviceVM);
            }
            else
            {
                if (commandName == "DataDelete")
                {
                    if (SlaveDeviceVM == null)
                    {
                        PromptMessage.Message = "请先选择一条数据再执行删除操作";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (SlaveDeviceVM.Id == Guid.Empty)
                    {
                        SlaveDevicesVM.Remove(SlaveDevicesVM.Single(x => x.Id == Guid.Empty));
                    }
                    else
                    {

                        SlaveDeviceVM.SoftDeleted = true;
                        await slavedeviceRepository.UpdateAsync(SlaveDeviceVM.DomainModel);
                    }
                }
                else if (commandName == "DataSave")
                {
                    if (SlaveDeviceVM == null)
                    {
                        PromptMessage.Message = "没有需要保存的数据";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(SlaveDeviceVM.MasterCode))
                    {
                        PromptMessage.Message = "主设备编码不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (slavedeviceRepository.NoTrackingQuery().Any(x => x.Id != SlaveDeviceVM.Id && x.SlaveCode == SlaveDeviceVM.SlaveCode))
                    {
                        PromptMessage.Message = "从设备编码重复，请检查后重试";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (SlaveDeviceVM.Id == Guid.Empty)
                    {
                        await slavedeviceRepository.AddAsync(SlaveDeviceVM.DomainModel);
                    }
                    else
                    {
                        await slavedeviceRepository.UpdateAsync(SlaveDeviceVM.DomainModel);
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
