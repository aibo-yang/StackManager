using System.Collections;
using System.ComponentModel;
using Common.UI.WPF.PropertyGrid.Converters;

namespace Common.UI.WPF.PropertyGrid
{
    public abstract class PropertyDefinitionBase : LockedDependencyObject
    {
        private IList targetProperties;
        [TypeConverter(typeof(ListConverter))]
        public IList TargetProperties
        {
            get { return targetProperties; }
            set
            {
                this.ThrowIfLocked(() => this.TargetProperties);
                targetProperties = value;
            }
        }

        private PropertyDefinitionCollection propertyDefinitions;
        public PropertyDefinitionCollection PropertyDefinitions
        {
            get
            {
                return propertyDefinitions;
            }
            set
            {
                this.ThrowIfLocked(() => this.PropertyDefinitions);
                propertyDefinitions = value;
            }
        }

        internal PropertyDefinitionBase()
        {
        }
    }
}
