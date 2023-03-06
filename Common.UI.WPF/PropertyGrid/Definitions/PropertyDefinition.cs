namespace Common.UI.WPF.PropertyGrid
{
    public class PropertyDefinition : PropertyDefinitionBase
    {
        private string category;
        public string Category
        {
            get { return category; }
            set
            {
                this.ThrowIfLocked(() => this.Category);
                category = value;
            }
        }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                this.ThrowIfLocked(() => this.DisplayName);
                displayName = value;
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                this.ThrowIfLocked(() => this.Description);
                description = value;
            }
        }

        private int? displayOrder;
        public int? DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                this.ThrowIfLocked(() => this.DisplayOrder);
                displayOrder = value;
            }
        }

        private bool? isBrowsable;
        public bool? IsBrowsable
        {
            get { return isBrowsable; }
            set
            {
                this.ThrowIfLocked(() => this.IsBrowsable);
                isBrowsable = value;
            }
        }

        public bool? isExpandable;
        public bool? IsExpandable
        {
            get { return isExpandable; }
            set
            {
                this.ThrowIfLocked(() => this.IsExpandable);
                isExpandable = value;
            }
        }

        internal override void Lock()
        {
            base.Lock();
        }
    }
}
