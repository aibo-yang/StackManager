using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using StackManager.Context.Domain;

namespace StackManager.Context.UI
{
    abstract class VmCollection<VM, DM> : ObservableCollection<VM> where DM:IEntity
    {
        private IList<DM> domainCollection;
        private bool eventsDisabled;

        public VmCollection(IList<DM> domainCollection)
        {
            this.domainCollection = domainCollection;
            this.eventsDisabled = true;

            foreach (var DomainModel in domainCollection)
            {
                var paramList = new object[] { DomainModel };
                var wrapperObject = (VM)Activator.CreateInstance(typeof(VM), paramList);
                this.Add(wrapperObject);
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
