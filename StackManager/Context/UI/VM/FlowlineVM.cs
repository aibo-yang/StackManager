using System;
using System.Collections.Generic;
using StackManager.Context.Domain;
using StackManager.Context.UI;

namespace StackManager.UI
{
    class FlowlinesVM : VmCollection<FlowlineVM, Flowline>
    {
        public FlowlinesVM(IList<Flowline> domainCollection) : base(domainCollection)
        {
        }
    }

    class FlowlineVM : BaseVM<Flowline>
    {
        public FlowlineVM() : this(new Flowline())
        {
        }

        public FlowlineVM(Flowline domainModel) : base(domainModel)
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
            get { return DomainModel.Index +1; }
            set
            {
                DomainModel.Index = value;
                RaisePropertyChangedEvent(nameof(Index));
            }
        }

        private DeviceCategoryVM elevator;
        public DeviceCategoryVM Elevator
        {
            get
            {
                if (elevator == null)
                {
                    elevator = new DeviceCategoryVM(DomainModel.Elevator);
                }
                return elevator;
            }
            set
            { 
                elevator = value;
                RaisePropertyChangedEvent(nameof(Elevator));
            }
        }
    }
}
