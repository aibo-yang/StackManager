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
    class ProductEditViewModel : DialogViewModel
    {
        private readonly ILogger<ProductEditViewModel> logger;
        private readonly IEventAggregator eventAggregator;
        private readonly IUnitOfWork unitOfWork;

        readonly IRepository<ProductCategory> productCategoryRepository;
        readonly IRepository<Box> boxRepository;

        public DelegateCommand<string> ButtonCommands { get; set; }

        private ProductCategoriesVM productCategoriesVM;
        public ProductCategoriesVM ProductCategoriesVM
        { 
            get { return productCategoriesVM; }
            set { SetProperty(ref productCategoriesVM, value); }
        }

        private ProductCategoryVM productCategoryVM;
        public ProductCategoryVM ProductCategoryVM
        {
            get { return productCategoryVM; }
            set { SetProperty(ref productCategoryVM, value); }
        }

        public ProductEditViewModel(ILogger<ProductEditViewModel> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork)
        {
            Title = "配置箱体信息";
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;

            this.productCategoryRepository = this.unitOfWork.GetRepository<ProductCategory>();
            this.boxRepository = this.unitOfWork.GetRepository<Box>();
            ButtonCommands = new DelegateCommand<string>(ButtonCommandsClick);

            RefershPageView();
        }

        void RefershPageView()
        {
            unitOfWork.ClearDbContext();
            ProductCategoryVM = null;
            ProductCategoriesVM = new ProductCategoriesVM(this.productCategoryRepository.NoTrackingQuery().ToList());
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
                if (ProductCategoriesVM.Any(x => x.Id == Guid.Empty))
                {
                    PromptMessage.Message = "请先保存修改后的数据再执行新增操作";
                    PromptMessage.HasError = true;
                    return;
                }
                ProductCategoryVM = new ProductCategoryVM();
                ProductCategoriesVM.Add(ProductCategoryVM);
            }
            else 
            {
                if (commandName == "DataDelete")
                {
                    if (ProductCategoryVM == null)
                    {
                        PromptMessage.Message = "请先选择一条数据再执行删除操作";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (ProductCategoryVM.Id == Guid.Empty)
                    {
                        ProductCategoriesVM.Remove(ProductCategoriesVM.Single(x => x.Id == Guid.Empty));
                    }
                    else
                    {
                        if (await boxRepository.ExistsAsync(x => x.ProductCategory != null && x.ProductCategory == ProductCategoryVM.DomainModel))
                        {
                            PromptMessage.Message = "当前配方正在使用，请先清空箱子后再删除";
                            PromptMessage.HasError = true;
                            return;
                        }

                        ProductCategoryVM.SoftDeleted = true;
                        await productCategoryRepository.UpdateAsync(ProductCategoryVM.DomainModel);
                    }
                }
                else if (commandName == "DataSave")
                {
                    if (ProductCategoryVM == null)
                    {
                        PromptMessage.Message = "没有需要保存的数据";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (ProductCategoryVM.Name != null)
                    {
                        ProductCategoryVM.Name = ProductCategoryVM.Name.Trim();
                    }

                    if (string.IsNullOrEmpty(ProductCategoryVM.Name))
                    {
                        PromptMessage.Message = "产品名称不能为空";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (ProductCategoryVM.PLCCode <= 0)
                    {
                        PromptMessage.Message = "PLC编码必须大于零";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (ProductCategoryVM.PalletBoxCount <= 0)
                    {
                        PromptMessage.Message = "栈板箱子数必须大于零";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (ProductCategoryVM.BoxRow <= 0)
                    {
                        PromptMessage.Message = "箱子行数必须大于零";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (ProductCategoryVM.BoxCol <= 0)
                    {
                        PromptMessage.Message = "箱子列数必须大于零";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (ProductCategoryVM.Index <= 0)
                    {
                        PromptMessage.Message = "箱子序号必须大于零";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (productCategoryRepository.NoTrackingQuery().Any(x => x.Id != ProductCategoryVM.Id && x.Name == ProductCategoryVM.Name))
                    {
                        PromptMessage.Message = "产品名称重复，请检查后重试";
                        PromptMessage.HasError = true;
                        return;
                    }

                    if (ProductCategoryVM.Id == Guid.Empty)
                    {
                        await productCategoryRepository.AddAsync(ProductCategoryVM.DomainModel);
                    }
                    else
                    {
                        await productCategoryRepository.UpdateAsync(ProductCategoryVM.DomainModel);
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
                    PromptMessage.Message = "保存箱子数据失败，请重试";
                    PromptMessage.HasError = true;
                }
            }
        }
    }
}