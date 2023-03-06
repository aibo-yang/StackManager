using System;
using Prism.Mvvm;
using StackManager.Context.Domain;
using StackManager.Context.UI;

namespace StackManager.Context.UI
{
    class BaseVM<T> : VmObject<T> where T : IEntity, new()
    {
        public Guid Id
        {
            get { return DomainModel.Id; }
        }

        public bool SoftDeleted
        {
            get { return DomainModel.SoftDeleted; }
            set
            {
                DomainModel.SoftDeleted = value;
                RaisePropertyChangedEvent(nameof(SoftDeleted));
            }
        }

        public BaseVM() : this(new T())
        {
        }

        public BaseVM(T domainModel) : base(domainModel)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is BaseVM<T> that)
            {
                if (Equals(Id, Guid.Empty) && Equals(that.Id, Guid.Empty))
                {
                    return ReferenceEquals(this, that);
                }
                return Id.Equals(that.Id);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            if (Equals(Id, Guid.Empty))
            {
                return base.GetHashCode();
            }
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseVM<T> left, BaseVM<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseVM<T> left, BaseVM<T> right)
        {
            return !Equals(left, right);
        }
    }
}
