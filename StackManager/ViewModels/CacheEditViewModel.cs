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
    class CacheEditViewModel : DialogViewModel
    {
        private readonly ILogger<CacheEditViewModel> logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IUnitOfWork unitOfWork;

        readonly IRepository<Box> boxRepository;
        readonly IRepository<ProductCategory> productCategoryRepository;
        readonly IRepository<Flowline> flowlineRepository;

        public DelegateCommand<string> ButtonCommands { get; set; }

        private BoxesVM boxesVM;
        public BoxesVM BoxesVM
        {
            get { return boxesVM; }
            set { SetProperty(ref boxesVM, value); }
        }

        private BoxVM boxVM;
        public BoxVM BoxVM
        {
            get { return boxVM; }
            set { SetProperty(ref boxVM, value); }
        }

        private ProductCategoriesVM productCategoriesVM;
        public ProductCategoriesVM ProductCategoriesVM
        {
            get { return productCategoriesVM; }
            set { SetProperty(ref productCategoriesVM, value); }
        }

        private FlowlinesVM flowlinesVM;
        public FlowlinesVM FlowlinesVM
        {
            get { return flowlinesVM; }
            set { SetProperty(ref flowlinesVM, value); }
        }

        public CacheEditViewModel(ILogger<CacheEditViewModel> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            Title = "管理缓存信息";
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.boxRepository = this.unitOfWork.GetRepository<Box>();
            this.productCategoryRepository = this.unitOfWork.GetRepository<ProductCategory>();
            this.flowlineRepository = this.unitOfWork.GetRepository<Flowline>();
         
            ButtonCommands = new DelegateCommand<string>(ButtonCommandsClick);

            ProductCategoriesVM = new ProductCategoriesVM(productCategoryRepository.NoTrackingQuery().ToList());
            FlowlinesVM = new FlowlinesVM(flowlineRepository.NoTrackingQuery().ToList());

            RefershPageView();
        }

        void RefershPageView()
        {
            unitOfWork.ClearDbContext();
            BoxVM = null;

            BoxesVM = new BoxesVM(this.boxRepository
                .NoTrackingQuery()
                .Include(x => x.ProductCategory)
                .Include(x => x.Flowline)
                .Where(x=>x.Status == BoxStatus.Caching)
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
                    if (BoxVM == null)
                    {
                        PromptMessage.Message = "请先选择一条数据再执行删除操作";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (BoxVM.Id == Guid.Empty)
                    {
                    }
                    else
                    {
                        BoxVM.SoftDeleted = true;
                        await boxRepository.UpdateAsync(BoxVM.DomainModel);
                    }
                }
                else if (commandName == "DataSave")
                {
                   
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
                    PromptMessage.Message = "删除缓存数据失败，请重试";
                    PromptMessage.HasError = true;
                }
            }
        }
    }
}
