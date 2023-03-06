using System.Collections;
using System.Collections.Specialized;
using System.Windows.Data;

namespace Common.UI.WPF.Charts
{
    internal class ItemsCollectionView : CollectionView
    {
        public override bool CanFilter => false;
        public override bool CanGroup => false;
        public override bool CanSort => false;

        public ItemsCollectionView(IEnumerable list) : base(list)
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnCollectionChanged(args);
            ChangedEvent?.Invoke(this, args);
        }

        public event NotifyCollectionChangedEventHandler ChangedEvent;
    }
}
