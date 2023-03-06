using System;
using Prism.Mvvm;

namespace StackManager.UI
{
    public class RobotInfo : BindableBase
    {
        private int stackingCount;
        public int StackingCount
        {
            get { return stackingCount; }
            set { SetProperty(ref stackingCount, value); }
        }

        private int waitingCount;
        public int WaitingCount
        {
            get { return waitingCount; }
            set { SetProperty(ref waitingCount, value); }
        }
    }
}
