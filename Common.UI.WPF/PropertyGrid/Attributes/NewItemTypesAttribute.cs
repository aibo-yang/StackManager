using System;
using System.Collections.Generic;

namespace Common.UI.WPF.PropertyGrid.Attributes
{
    public class NewItemTypesAttribute : Attribute
    {
        public IList<Type> Types
        {
            get;
            set;
        }

        public NewItemTypesAttribute(params Type[] types)
        {
            this.Types = new List<Type>(types);
        }

        public NewItemTypesAttribute()
        {
        }
    }
}
