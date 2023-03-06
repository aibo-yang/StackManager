using System;
using System.Collections.Generic;
using StackManager.Context.Domain;
using StackManager.Context.UI;

namespace StackManager.UI
{
    class BoxesVM : VmCollection<BoxVM, Box>
    {
        public BoxesVM(IList<Box> domainCollection) : base(domainCollection)
        {
        }
    }

    class BoxVM : BaseVM<Box>
    {
        public BoxVM() : this(new Box())
        {
        }

        public BoxVM(Box domainModel) : base(domainModel)
        {
        }

        public string Code
        {
            get { return DomainModel.Code; }
            set
            {
                DomainModel.Code = value;
                RaisePropertyChangedEvent(nameof(Code));
            }
        }

        public BoxStatus Status
        {
            get { return DomainModel.Status; }
            set
            {
                DomainModel.Status = value;
                RaisePropertyChangedEvent(nameof(Status));
            }
        }

        public bool BoxIsFull
        {
            get { return DomainModel.BoxIsFull; }
            set
            {
                DomainModel.BoxIsFull = value;
                RaisePropertyChangedEvent(nameof(BoxIsFull));
            }
        }

        public bool PalletIsFull
        {
            get { return DomainModel.PalletIsFull; }
            set
            {
                DomainModel.PalletIsFull = value;
                RaisePropertyChangedEvent(nameof(PalletIsFull));
            }
        }

        public string PalletNo
        {
            get { return DomainModel.PalletNo; }
            set
            {
                DomainModel.PalletNo = value;
                RaisePropertyChangedEvent(nameof(PalletNo));
            }
        }

        public string OrderNo
        {
            get { return DomainModel.OrderNo; }
            set
            {
                DomainModel.OrderNo = value;
                RaisePropertyChangedEvent(nameof(OrderNo));
            }
        }

        private ProductCategoryVM productCategory;
        public ProductCategoryVM ProductCategory
        {
            get
            {
                if (productCategory == null)
                {
                    productCategory = new ProductCategoryVM(DomainModel.ProductCategory);
                }
                return productCategory;
            }
            set
            {
                productCategory = value;
                RaisePropertyChangedEvent(nameof(ProductCategory));
            }
        }

        private FlowlineVM flowline;
        public FlowlineVM Flowline
        {
            get
            {
                if (flowline == null)
                {
                    flowline = new FlowlineVM(DomainModel.Flowline);
                }
                return flowline;
            }
            set
            {
                flowline = value;
                RaisePropertyChangedEvent(nameof(Flowline));
            }
        }
    }
}
