using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Common.Toolkits.Entities
{
    public abstract class VmCollection<VM, DM> : ObservableCollection<VM> 
        where DM : BaseDomain 
        where VM : VmObject<DM>
    {
        private ICollection<DM> domainCollection;
        private bool eventsDisabled;

        public VmCollection(ICollection<DM> domainCollection)
        {
            this.domainCollection = domainCollection;
            this.eventsDisabled = true;

            foreach (var domainModel in domainCollection)
            {
                var paramList = new object[] { domainModel };
                var wrapperObject = (VM)Activator.CreateInstance(typeof(VM), paramList);
                this.Add(wrapperObject);
            }
            this.eventsDisabled = false;
        }

        public VmCollection(ICollection<VM> vmCollection)
        {
            this.domainCollection = vmCollection.Select(x => x.DomainModel).ToList();
            this.eventsDisabled = true;

            foreach (var vmModel in vmCollection)
            {
                this.Add(vmModel);
            }
            this.eventsDisabled = false;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (eventsDisabled)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddDomainModels(e);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveDomainModels(e);
                    break;
            }
        }

        private void AddDomainModels(NotifyCollectionChangedEventArgs e)
        {
            foreach (VmObject<DM> wrapperObject in e.NewItems)
            {
                var DomainModel = wrapperObject.DomainModel;
                domainCollection.Add(DomainModel);
            }
        }

        private void RemoveDomainModels(NotifyCollectionChangedEventArgs e)
        {
            foreach (VmObject<DM> wrapperObject in e.OldItems)
            {
                var DomainModel = wrapperObject.DomainModel;
                domainCollection.Remove(DomainModel);
            }
        }
    }
}
