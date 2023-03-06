using System;
using StackManager.Context.Domain;

namespace StackManager.Context.UI
{
    abstract class VmObject<DM> : ViewModelBase where DM : IEntity
    {
        protected VmObject(DM domainModel)
        {
            this.DomainModel = domainModel;
        }

        internal DM DomainModel { get; set; }
    }
}
