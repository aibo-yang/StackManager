using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace StackManager.Behaviors
{
    public class AutoScrollHandler : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(AutoScrollHandler),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.None,
                ItemsSourcePropertyChanged));

        private bool canScroll = false;
        private ListBox listBox;
        private DispatcherTimer listBoxTimer;
        private int listBoxIndex = 0;

        public AutoScrollHandler(System.Windows.Controls.ListBox target)
        {
            this.listBox = target;
            var binding = new Binding("ItemsSource") { Source = this.listBox };
            BindingOperations.SetBinding(this, ItemsSourceProperty, binding);

            listBoxTimer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(2000)
            };

            listBoxTimer.Tick += (sender, args) =>
            {
                if (!canScroll)
                {
                    return;
                }

                if (listBoxIndex >= listBox.Items.Count)
                {
                    listBoxIndex = 0;
                }

                Debug.WriteLine($"{DateTime.Now},{listBoxIndex}");
                listBox.ScrollIntoView(listBox.Items[listBoxIndex++]);
            };

            listBoxTimer.Start();
        }

        public void Dispose()
        {
            listBoxTimer.Stop();
            BindingOperations.ClearBinding(this, ItemsSourceProperty);
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((AutoScrollHandler)o).ItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void ItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            var collection = oldValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged -= this.CollectionChangedEventHandler;
            }

            collection = newValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged += this.CollectionChangedEventHandler;
            }
        }

        private void CollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems == null || e.NewItems.Count < 1)
            {
                // listBoxTimer.Stop();
                canScroll = false;
                return;
            }

            canScroll = listBox.Items.Count > 5;
            // listBoxTimer.Start();
            // this.target.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
        }
    }
}
