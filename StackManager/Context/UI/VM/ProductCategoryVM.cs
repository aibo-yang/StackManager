using System.Collections.Generic;
using StackManager.Context.Domain;
using StackManager.Context.UI;

namespace StackManager.UI
{
    class ProductCategoriesVM : VmCollection<ProductCategoryVM, ProductCategory>
    {
        public ProductCategoriesVM(IList<ProductCategory> domainCollection) : base(domainCollection)
        {
        }
    }

    class ProductCategoryVM : BaseVM<ProductCategory>
    {
        public ProductCategoryVM() : this(new ProductCategory())
        {
        }

        public ProductCategoryVM(ProductCategory domainModel) : base(domainModel)
        {
        }

        public string Name
        {
            get { return DomainModel.Name; }
            set
            {
                DomainModel.Name = value;
                RaisePropertyChangedEvent(nameof(Name));
            }
        }

        public string BoxCode
        {
            get { return DomainModel.BoxCode; }
            set
            {
                DomainModel.BoxCode = value;
                RaisePropertyChangedEvent(nameof(BoxCode));
            }
        }

        public ushort PLCCode
        {
            get { return DomainModel.PLCCode; }
            set
            {
                DomainModel.PLCCode = value;
                RaisePropertyChangedEvent(nameof(PLCCode));
            }
        }

        public int Rate
        {
            get { return DomainModel.Rate; }
            set
            {
                DomainModel.Rate = value;
                RaisePropertyChangedEvent(nameof(Rate));
            }
        }

        //0719
        public int Index
        {
            get { return DomainModel.Index; }
            set
            {
                DomainModel.Index = value;
                RaisePropertyChangedEvent(nameof(Index));
            }
        }

        public int BoxProductCount
        {
            get { return DomainModel.BoxProductCount; }
            set
            {
                DomainModel.BoxProductCount = value;
                RaisePropertyChangedEvent(nameof(BoxProductCount));
            }
        }

        public int PalletBoxCount
        {
            get { return DomainModel.PalletBoxCount; }
            set
            {
                DomainModel.PalletBoxCount = value;
                RaisePropertyChangedEvent(nameof(PalletBoxCount));
            }
        }

        public int BoxRow
        {
            get { return DomainModel.BoxRow; }
            set
            {
                DomainModel.BoxRow = value;
                RaisePropertyChangedEvent(nameof(BoxRow));
            }
        }

        public int BoxCol
        {
            get { return DomainModel.BoxCol; }
            set
            {
                DomainModel.BoxCol = value;
                RaisePropertyChangedEvent(nameof(BoxCol));
            }
        }

        public int BoxHeight
        {
            get { return DomainModel.BoxHeight; }
            set
            {
                DomainModel.BoxHeight = value;
                RaisePropertyChangedEvent(nameof(BoxHeight));
            }
        }

        public int BoxWidth
        {
            get { return DomainModel.BoxWidth; }
            set
            {
                DomainModel.BoxWidth = value;
                RaisePropertyChangedEvent(nameof(BoxWidth));
            }
        }

        public int BoxLength
        {
            get { return DomainModel.BoxLength; }
            set
            {
                DomainModel.BoxLength = value;
                RaisePropertyChangedEvent(nameof(BoxLength));
            }
        }

        public ushort PalletType
        {
            get { return DomainModel.PalletType; }
            set
            {
                DomainModel.PalletType = value;
                RaisePropertyChangedEvent(nameof(PalletType));
            }
        }

        public ushort LayoutType
        {
            get { return DomainModel.LayoutType; }
            set
            {
                DomainModel.LayoutType = value;
                RaisePropertyChangedEvent(nameof(LayoutType));
            }
        }

        public ushort BoxBoard
        {
            get { return DomainModel.BoxBoard; }
            set
            {
                DomainModel.BoxBoard = value;
                RaisePropertyChangedEvent(nameof(BoxBoard));
            }
        }

        public ushort StackType 
        {
            get { return DomainModel.StackType; }
            set 
            {
                DomainModel.StackType = value;
                RaisePropertyChangedEvent(nameof(StackType));
            }
        }

        public ushort CacheRegion
        {
            get { return DomainModel.CacheRegion; }
            set
            {
                DomainModel.CacheRegion = value;
                RaisePropertyChangedEvent(nameof(CacheRegion));
            }
        }
    }
}
