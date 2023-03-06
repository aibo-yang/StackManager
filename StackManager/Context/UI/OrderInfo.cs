using System;
using Prism.Mvvm;

namespace StackManager.UI
{
    public class OrderInfo : BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string lineName;
        public string LineName
        {
            get { return lineName; }
            set { SetProperty(ref lineName, value); }
        }

        private string type;
        public string Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { SetProperty(ref count, value); }
        }

        //private double progress;
        //public double Progress
        //{
        //    get { return progress; }
        //    set { SetProperty(ref progress, value); }
        //}

        //private string status;
        //public string Status
        //{
        //    get { return status; }
        //    set { SetProperty(ref status, value); }
        //}

        //private string opCode;
        //public string OPCode
        //{
        //    get { return opCode; }
        //    set { SetProperty(ref opCode, value); }
        //}

        //private string oiRate;
        //public string OIRate
        //{
        //    get { return oiRate; }
        //    set { SetProperty(ref oiRate, value); }
        //}

        public DateTime startDateTime;
        public string StartDateTime
        {
            get { return startDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { SetProperty(ref startDateTime, DateTime.Parse(value)); }
        }

        //public DateTime endDateTime;
        //public string EndDateTime
        //{
        //    get { return endDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        //    set { SetProperty(ref endDateTime, DateTime.Parse(value)); }
        //}
    }
}
