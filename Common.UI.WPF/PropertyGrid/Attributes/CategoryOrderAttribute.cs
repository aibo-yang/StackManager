using System;

namespace Common.UI.WPF.PropertyGrid.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CategoryOrderAttribute : Attribute
    {
        public int Order
        {
            get;
            set;
        }

        public virtual string Category
        {
            get
            {
                return CategoryValue;
            }
        }

        public string CategoryValue
        {
            get;
            private set;
        }

        public override object TypeId
        {
            get
            {
                return this.CategoryValue;
            }
        }

        public CategoryOrderAttribute()
        {
        }

        public CategoryOrderAttribute(string categoryName, int order)
          : this()
        {
            CategoryValue = categoryName;
            Order = order;
        }
    }
}
