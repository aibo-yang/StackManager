using System;

namespace Common.UI.WPF.Charts
{
    public class DataRange
    {
        private double end;
        public double End
        {
            get
            {
                return end;
            }
            set
            {
                if (end != value)
                {
                    end = value;
                    isEmpty = false;
                }
            }
        }

        private bool isEmpty = true;
        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }
        }

        public double Length
        {
            get
            {
                return End - Start;
            }
        }

        private double start;
        public double Start
        {
            get
            {
                return start;
            }
            set
            {
                if (start != value)
                {
                    start = value;
                    isEmpty = false;
                }
            }
        }

        public DataRange()
        {
            this.isEmpty = true;
        }

        public DataRange(double start, double end)
        {
            this.isEmpty = false;
            this.start = Math.Min(start, end);
            this.end = Math.Max(start, end);
        }

        public bool Contains(double value)
        {
            bool flag = false;
            if (!this.IsEmpty && this.Start <= value && this.End >= value)
            {
                flag = true;
            }
            return flag;
        }

        public override bool Equals(object o)
        {
            if (o is not DataRange)
            {
                return false;
            }

            DataRange dataRange = (DataRange)o;
            if (this.IsEmpty)
            {
                return dataRange.IsEmpty;
            }

            if (!object.Equals(this.Start, dataRange.Start))
            {
                return false;
            }

            return object.Equals(this.End, dataRange.End);
        }

        public override int GetHashCode()
        {
            return this.isEmpty.GetHashCode() ^ this.start.GetHashCode() ^ this.end.GetHashCode();
        }

        public static DataRange operator +(DataRange r1, DataRange r2)
        {
            var dataRange = new DataRange();

            if (r1.IsEmpty)
            {
                dataRange = r2;
            }
            else if (!r2.IsEmpty)
            {
                dataRange = new DataRange(Math.Min(r1.Start, r2.Start), Math.Max(r1.End, r2.End));
            }
            else
            {
                dataRange = r1;
            }

            return dataRange;
        }

        public static bool operator ==(DataRange r1, DataRange r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(DataRange r1, DataRange r2)
        {
            return !r1.Equals(r2);
        }

        public void Reset()
        {
            this.start = 0;
            this.end = 0;
            this.isEmpty = true;
        }
    }
}
