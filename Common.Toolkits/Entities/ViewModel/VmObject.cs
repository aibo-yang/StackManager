namespace Common.Toolkits.Entities
{
    public abstract class VmObject<DM> : ViewModelBase where DM : BaseDomain
    {
        protected VmObject(DM domainModel)
        {
            this.DomainModel = domainModel;
        }

        public DM DomainModel { get;}
    }
}
