﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Common.UI.WPF.PropertyGrid
{
    public class PropertyItemsControl : ItemsControl
    {
        internal static readonly RoutedEvent PreparePropertyItemEvent = EventManager.RegisterRoutedEvent("PreparePropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyItemsControl));
        internal event PropertyItemEventHandler PreparePropertyItem
        {
            add
            {
                AddHandler(PropertyItemsControl.PreparePropertyItemEvent, value);
            }
            remove
            {
                RemoveHandler(PropertyItemsControl.PreparePropertyItemEvent, value);
            }
        }

        private void RaisePreparePropertyItemEvent(PropertyItemBase propertyItem, object item)
        {
            this.RaiseEvent(new PropertyItemEventArgs(PropertyItemsControl.PreparePropertyItemEvent, this, propertyItem, item));
        }

        internal static readonly RoutedEvent ClearPropertyItemEvent = EventManager.RegisterRoutedEvent("ClearPropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyItemsControl));
        internal event PropertyItemEventHandler ClearPropertyItem
        {
            add
            {
                AddHandler(PropertyItemsControl.ClearPropertyItemEvent, value);
            }
            remove
            {
                RemoveHandler(PropertyItemsControl.ClearPropertyItemEvent, value);
            }
        }

        private void RaiseClearPropertyItemEvent(PropertyItemBase propertyItem, object item)
        {
            this.RaiseEvent(new PropertyItemEventArgs(PropertyItemsControl.ClearPropertyItemEvent, this, propertyItem, item));
        }

        public PropertyItemsControl()
        {
            var propertyItemsControlProperties = TypeDescriptor.GetProperties(this, new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) });
           
            var prop1 = propertyItemsControlProperties.Find("VirtualizingPanel.IsVirtualizingWhenGrouping", false);
            if (prop1 != null)
            {
                prop1.SetValue(this, true);
            }
           
            var prop2 = propertyItemsControlProperties.Find("VirtualizingPanel.CacheLengthUnit", false);
            if (prop2 != null)
            {
                prop2.SetValue(this, Enum.ToObject(prop2.PropertyType, 1));
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is PropertyItemBase);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            this.RaisePreparePropertyItemEvent((PropertyItemBase)element, item);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            this.RaiseClearPropertyItemEvent((PropertyItemBase)element, item);
            base.ClearContainerForItemOverride(element, item);
        }
    }
}
