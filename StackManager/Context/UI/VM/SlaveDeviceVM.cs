using System;
using System.Collections.Generic;
using StackManager.Context.Domain;
using StackManager.Context.UI;

namespace StackManager.Context.UI
{

    class SlaveDevicesVM : VmCollection<SlaveDeviceVM, SlaveDevice>
    {
        public SlaveDevicesVM(IList<SlaveDevice> domainCollection) : base(domainCollection)
        {
        }
    }

    class SlaveDeviceVM :BaseVM<SlaveDevice>
    {
        public SlaveDeviceVM() : this(new SlaveDevice())
        {
        }

        public SlaveDeviceVM(SlaveDevice domainModel) : base(domainModel)
        {
        }

        public string SlaveCode
        {
            get { return DomainModel.SlaveCode; }
            set
            {
                DomainModel.SlaveCode = value;
                RaisePropertyChangedEvent(nameof(SlaveCode));
            }
        }

        public string MasterCode
        {
            get { return DomainModel.MasterCode; }
            set
            {
                DomainModel.MasterCode = value;
                RaisePropertyChangedEvent(nameof(MasterCode));
            }
        }
    }
}
