using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using StackManager.Context.Domain;
using StackManager.Extensions;
using StackManager.Repositories;
using StackManager.UI;

namespace StackManager.ViewModels
{
    class FlowlineEditViewModel : DialogViewModel
    {
        private readonly ILogger<FlowlineEditViewModel> logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IUnitOfWork unitOfWork;

        readonly IRepository<Flowline> flowlineRepository;
        readonly IRepository<DeviceCategory> deviceCategoryeRepository;

        public DelegateCommand<string> ButtonCommands { get; set; }

        private FlowlinesVM flowlinesVM;
        public FlowlinesVM FlowlinesVM
        {
            get { return flowlinesVM; }
            set { SetProperty(ref flowlinesVM, value); }
        }

        private DeviceCategoriesVM elevatorsVM;
        public DeviceCategoriesVM ElevatorsVM
        {
            get { return elevatorsVM; }
            set { SetProperty(ref elevatorsVM, value); }
        }

        private FlowlineVM flowlineVM;
        public FlowlineVM FlowlineVM
        {
            get { return flowlineVM; }
            set { SetProperty(ref flowlineVM, value); }
        }

        public FlowlineEditViewModel(ILogger<FlowlineEditViewModel> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            Title = "配置产线信息";
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.flowlineRepository = this.unitOfWork.GetRepository<Flowline>();
            this.deviceCategoryeRepository = this.unitOfWork.GetRepository<DeviceCategory>();

            ButtonCommands = new DelegateCommand<string>(ButtonCommandsClick);

            ElevatorsVM = new DeviceCategoriesVM(this.deviceCategoryeRepository
                .NoTrackingQuery()
                .Where(x => x.DeviceType == DeviceType.ElevatorLoad).ToList());

            RefershPageView();
        }

        void RefershPageView()
        {
            unitOfWork.ClearDbContext();

            FlowlineVM = null;
            FlowlinesVM = new FlowlinesVM(flowlineRepository.NoTrackingQuery()
                .Include(x=>x.Elevator)
                .ToList());
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
                    if (FlowlineVM == null)
                    {
                        PromptMessage.Message = "没有需要保存的数据";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (FlowlineVM.Name != null)
                    {
                        FlowlineVM.Name = FlowlineVM.Name.Trim();
                    }

                    if (string.IsNullOrEmpty(FlowlineVM.Name))
                    {
                        PromptMessage.Message = "产线名称不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (flowlineRepository.NoTrackingQuery().Any(x => x.Id != FlowlineVM.Id && x.Name == FlowlineVM.Name))
                    {
                        PromptMessage.Message = "产线名称重复，请检查后重试";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (FlowlineVM.Id == Guid.Empty)
                    {
                    }
                    else
                    {
                        var updateVM = await flowlineRepository
                            .TrackingQuery()
                            .Include(x=>x.Elevator)
                            .SingleOrDefaultAsync(x=>x.Id == FlowlineVM.Id);

                        updateVM.Name = FlowlineVM.Name;

                        updateVM.Elevator = await deviceCategoryeRepository
                            .TrackingQuery()
                            .Include(x => x.Flowlines)
                            .SingleOrDefaultAsync(x=>x.Id == FlowlineVM.Elevator.Id);

                        await flowlineRepository.UpdateAsync(updateVM);
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
                    PromptMessage.Message = "保存产线数据失败，请重试";
                    PromptMessage.HasError = true;
                }
            }
        }
    }
}
