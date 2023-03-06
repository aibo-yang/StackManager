using System;
using System.Windows;

namespace Common.UI.WPF.PropertyGrid
{
    public class CustomPropertyItem : PropertyItemBase
    {
        internal CustomPropertyItem() { }

        internal CustomPropertyItem(bool isPropertyGridCategorized, bool isSortedAlphabetically)
        {
            base.isPropertyGridCategorized = isPropertyGridCategorized;
            base.isSortedAlphabetically = isSortedAlphabetically;
        }

        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(string), typeof(CustomPropertyItem), new UIPropertyMetadata(null));

        public string Category
        {
            get { return (string)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        public int CategoryOrder
        {
            get
            {
                return categoryOrder;
            }
            set
            {
                if (categoryOrder != value)
                {
                    categoryOrder = value;
                    
                    this.RaisePropertyChanged(() => this.CategoryOrder);
                }
            }
        }

        private int categoryOrder;
        public static readonly DependencyProperty PropertyOrderProperty =
            DependencyProperty.Register("PropertyOrder", typeof(int), typeof(CustomPropertyItem), new UIPropertyMetadata(0));

        public int PropertyOrder
        {
            get
            {
                return (int)GetValue(PropertyOrderProperty);
            }
            set
            {
                SetValue(PropertyOrderProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(CustomPropertyItem), new UIPropertyMetadata(null, OnValueChanged, OnCoerceValueChanged));
        public object Value
        {
            get
            {
                return (object)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        private static object OnCoerceValueChanged(DependencyObject o, object baseValue)
        {
            CustomPropertyItem prop = o as CustomPropertyItem;
            if (prop != null)
                return prop.OnCoerceValueChanged(baseValue);

            return baseValue;
        }

        protected virtual object OnCoerceValueChanged(object baseValue)
        {
            return baseValue;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomPropertyItem propertyItem = o as CustomPropertyItem;
            if (propertyItem != null)
            {
                propertyItem.OnValueChanged((object)e.OldValue, (object)e.NewValue);
            }
        }

        protected virtual void OnValueChanged(object oldValue, object newValue)
        {
            if (IsInitialized)
            {
                RaiseEvent(new PropertyValueChangedEventArgs(PropertyGrid.PropertyValueChangedEvent, this, oldValue, newValue));
            }
        }

        protected override Type GetPropertyItemType()
        {
            return this.Value.GetType();
        }

        protected override void OnEditorChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
            {
                oldValue.DataContext = null;
            }

            
            if ((newValue != null) && (newValue.DataContext == null))
            {
                newValue.DataContext = this;
            }
        }
    }
}
