using System;
using System.Collections.Generic;
using StackManager.Context.Domain;
using StackManager.Context.UI;

namespace StackManager.UI
{
    class SettingVM : BaseVM<Setting>
    {
        public SettingVM() : this(new Setting())
        {
        }

        public SettingVM(Setting domainModel) : base(domainModel)
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

        public string SerialNumber
        {
            get { return DomainModel.SerialNumber; }
            set
            {
                DomainModel.SerialNumber = value;
                RaisePropertyChangedEvent(nameof(SerialNumber));
            }
        }

        public string MesUri
        {
            get { return DomainModel.MesUri; }
            set
            {
                DomainModel.MesUri = value;
                RaisePropertyChangedEvent(nameof(MesUri));
            }
        }

        public string MesSecret
        {
            get { return DomainModel.MesSecret; }
            set
            {
                DomainModel.MesSecret = value;
                RaisePropertyChangedEvent(nameof(MesSecret));
            }
        }

        public string MesTokenID
        {
            get { return DomainModel.MesTokenId; }
            set
            {
                DomainModel.MesTokenId = value;
                RaisePropertyChangedEvent(nameof(MesTokenID));
            }
        }

        public string PqmUri
        {
            get { return DomainModel.PqmUri; }
            set
            {
                DomainModel.PqmUri = value;
                RaisePropertyChangedEvent(nameof(PqmUri));
            }
        }

        public string Password
        {
            get { return DomainModel.Password; }
            set
            {
                DomainModel.Password = value;
                RaisePropertyChangedEvent(nameof(Password));
            }
        }
    }
}
