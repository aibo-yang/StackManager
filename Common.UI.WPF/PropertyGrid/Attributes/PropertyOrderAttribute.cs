using System;

namespace Common.UI.WPF.PropertyGrid.Attributes
{
    public enum UsageContextEnum
    {
        Alphabetical,
        Categorized,
        Both
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class PropertyOrderAttribute : Attribute
    {
        public int Order
        {
            get;
            set;
        }

        public UsageContextEnum UsageContext
        {
            get;
            set;
        }

        public override object TypeId
        {
            get
            {
                return this;
            }
        }

        public PropertyOrderAttribute(int order)
          : this(order, UsageContextEnum.Both)
        {
        }

        public PropertyOrderAttribute(int order, UsageContextEnum usageContext)
        {
            Order = order;
            UsageContext = usageContext;
        }
    }
}
