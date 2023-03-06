using Prism.Mvvm;
using StackManager.Context.Domain;

namespace StackManager.UI
{
    class DeviceInfo : BindableBase
    {
        public int Index { get; set; }
        public DeviceType DeviceType { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string code;
        public string Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        private int status;
        public int Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private double activeTime;
        public double ActiveTime
        {
            get { return activeTime; }
            set { SetProperty(ref activeTime, value); }
        }

        private double alarmTime;
        public double AlarmTime
        {
            get { return alarmTime; }
            set { SetProperty(ref alarmTime, value); }
        }

        private double waitingTime;
        public double WaitingTime
        {
            get { return waitingTime; }
            set { SetProperty(ref waitingTime, value); }
        }

        private double efficency;
        public double Efficency
        {
            get { return efficency; }
            set { SetProperty(ref efficency, value); }
        }

        private int cycleTime;
        public int CycleTime
        {
            get { return cycleTime; }
            set { SetProperty(ref cycleTime, value); }
        }
    }
}
