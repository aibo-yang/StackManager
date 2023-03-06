using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace StackManager.Behaviors
{
    public class ListBoxAutoScrollBehavior : Behavior<ListBox>
    {
        private ScrollViewer scrollViewer;
        private bool autoScroll = true;
        private bool justWheeled = false;
        private bool userInteracting = false;
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            AssociatedObject.Unloaded += AssociatedObjectOnUnloaded;
        }

        private void AssociatedObjectOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged -= ScrollViewerOnScrollChanged;
            }
            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
            AssociatedObject.ItemContainerGenerator.ItemsChanged -= ItemContainerGeneratorItemsChanged;
            AssociatedObject.GotMouseCapture -= AssociatedObject_GotMouseCapture;
            AssociatedObject.LostMouseCapture -= AssociatedObject_LostMouseCapture;
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;

            scrollViewer = null;
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            scrollViewer = GetScrollViewer(AssociatedObject);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;

                AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
                AssociatedObject.ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorItemsChanged;
                AssociatedObject.GotMouseCapture += AssociatedObject_GotMouseCapture;
                AssociatedObject.LostMouseCapture += AssociatedObject_LostMouseCapture;
                AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
            }
        }

        private static ScrollViewer GetScrollViewer(DependencyObject root)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < childCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is ScrollViewer sv)
                {
                    return sv;
                }
                // return GetScrollViewer(child);
            }
            return null;
        }

        void AssociatedObject_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // User is actively interacting with listbox. Do not allow automatic scrolling to interfere with user experience.
            userInteracting = true;
            autoScroll = false;
        }

        void AssociatedObject_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // User is done interacting with control.
            userInteracting = false;
        }

        private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // diff is exactly zero if the last item in the list is visible. This can occur because of scroll-bar drag, mouse-wheel, or keyboard event.
            double diff = (scrollViewer.VerticalOffset - (scrollViewer.ExtentHeight - scrollViewer.ViewportHeight));

            // User just wheeled; this event is called immediately afterwards.
            if (justWheeled && diff != 0.0)
            {
                justWheeled = false;
                autoScroll = false;
                return;
            }

            if (diff == 0.0)
            {
                // then assume user has finished with interaction and has indicated through this action that scrolling should continue automatically.
                autoScroll = true;
            }
        }

        private void ItemContainerGeneratorItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset)
            {
                // An item was added to the listbox, or listbox was cleared.
                if (autoScroll && !userInteracting)
                {
                    // If automatic scrolling is turned on, scroll to the bottom to bring new item into view.
                    // Do not do this if the user is actively interacting with the listbox.
                    scrollViewer.ScrollToBottom();
                }
            }
        }

        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            // User selected (clicked) an item, or used the keyboard to select a different item. 
            // Turn off automatic scrolling.
            autoScroll = false;
        }

        void AssociatedObject_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // User wheeled the mouse. 
            // Cannot detect whether scroll viewer right at the bottom, because the scroll event has not occurred at this point.
            // Same for bubbling event.
            // Just indicated that the user mouse-wheeled, and that the scroll viewer should decide whether or not to stop autoscrolling.
            justWheeled = true;
        }
    }
}
