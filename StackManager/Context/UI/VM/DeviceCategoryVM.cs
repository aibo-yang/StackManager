using System;
using System.Collections.Generic;
using StackManager.Context.Domain;
using StackManager.Context.UI;

namespace StackManager.UI
{
    class DeviceCategoriesVM : VmCollection<DeviceCategoryVM, DeviceCategory>
    {
        public DeviceCategoriesVM(IList<DeviceCategory> domainCollection) : base(domainCollection)
        {
        }
    }

    class DeviceCategoryVM : BaseVM<DeviceCategory>
    {
        public DeviceCategoryVM() : this(new DeviceCategory())
        {
        }

        public DeviceCategoryVM(DeviceCategory domainModel) : base(domainModel)
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

        public int Index
        {
            get { return DomainModel.Index; }
            set
            {
                DomainModel.Index = value;
                RaisePropertyChangedEvent(nameof(Index));
            }
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
    }
}
