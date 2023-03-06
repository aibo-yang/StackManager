using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Common.UI.WPF.Core.Utilities;

namespace Common.UI.WPF.PropertyGrid
{
    [TemplatePart(Name = PropertyGrid.PART_PropertyItemsControl, Type = typeof(PropertyItemsControl))]
    [TemplatePart(Name = PropertyItemBase.PART_ValueContainer, Type = typeof(ContentControl))]
    public abstract class PropertyItemBase : Control, IPropertyContainer, INotifyPropertyChanged
    {
        internal const string PART_ValueContainer = "PART_ValueContainer";

        private ContentControl valueContainer;
        private ContainerHelper containerHelper;
        private IPropertyContainer parentNode;
        internal bool isPropertyGridCategorized;
        internal bool isSortedAlphabetically = true;

        public static readonly DependencyProperty AdvancedOptionsIconProperty =
            DependencyProperty.Register("AdvancedOptionsIcon", typeof(ImageSource), typeof(PropertyItemBase), new UIPropertyMetadata(null));
        public ImageSource AdvancedOptionsIcon
        {
            get { return (ImageSource)GetValue(AdvancedOptionsIconProperty); }
            set { SetValue(AdvancedOptionsIconProperty, value); }
        }

        public static readonly DependencyProperty AdvancedOptionsTooltipProperty =
            DependencyProperty.Register("AdvancedOptionsTooltip", typeof(object), typeof(PropertyItemBase), new UIPropertyMetadata(null));
        public object AdvancedOptionsTooltip
        {
            get { return (object)GetValue(AdvancedOptionsTooltipProperty); }
            set { SetValue(AdvancedOptionsTooltipProperty, value); }
        }
        
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(PropertyItemBase), new UIPropertyMetadata(null));
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(PropertyItemBase), new UIPropertyMetadata(null));
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty EditorProperty = DependencyProperty.Register("Editor", typeof(FrameworkElement), typeof(PropertyItemBase), new UIPropertyMetadata(null, OnEditorChanged));
        public FrameworkElement Editor
        {
            get
            {
                return (FrameworkElement)GetValue(EditorProperty);
            }
            set
            {
                SetValue(EditorProperty, value);
            }
        }

        private static void OnEditorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyItemBase propertyItem = o as PropertyItemBase;
            if (propertyItem != null)
            { 
                propertyItem.OnEditorChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
            }
        }

        protected virtual void OnEditorChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
        }

        public static readonly DependencyProperty HighlightedTextProperty = DependencyProperty.Register("HighlightedText", typeof(string), typeof(PropertyItemBase), new UIPropertyMetadata(null));
        public string HighlightedText
        {
            get
            {
                return (string)GetValue(HighlightedTextProperty);
            }
            set
            {
                SetValue(HighlightedTextProperty, value);
            }
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(PropertyItemBase), new UIPropertyMetadata(false, OnIsExpandedChanged));
        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }
            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        private static void OnIsExpandedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyItemBase propertyItem = o as PropertyItemBase;
            if (propertyItem != null)
            {
                propertyItem.OnIsExpandedChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsExpandedChanged(bool oldValue, bool newValue)
        {
        }

        public static readonly DependencyProperty IsExpandableProperty =
            DependencyProperty.Register("IsExpandable", typeof(bool), typeof(PropertyItemBase), new UIPropertyMetadata(false));

        public bool IsExpandable
        {
            get { return (bool)GetValue(IsExpandableProperty); }
            set { SetValue(IsExpandableProperty, value); }
        }
        
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(PropertyItemBase), new UIPropertyMetadata(false, OnIsSelectedChanged));
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        private static void OnIsSelectedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertyItemBase propertyItem = o as PropertyItemBase;
            if (propertyItem != null)
            {
                propertyItem.OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            this.RaiseItemSelectionChangedEvent();
        }

        public FrameworkElement ParentElement
        {
            get { return this.ParentNode as FrameworkElement; }
        }

        internal IPropertyContainer ParentNode
        {
            get
            {
                return parentNode;
            }
            set
            {
                parentNode = value;
            }
        }
        
        internal ContentControl ValueContainer
        {
            get
            {
                return valueContainer;
            }
        }

        public int Level
        {
            get;
            internal set;
        }
        
        public IList Properties
        {
            get
            {
                if (containerHelper == null)
                {
                    containerHelper = new ObjectContainerHelper(this, null);
                }
                return containerHelper.Properties;
            }
        }
        
        public Style PropertyContainerStyle
        {
            get
            {
                return (ParentNode != null) ? ParentNode.PropertyContainerStyle : null;
            }
        }
        
        internal ContainerHelper ContainerHelper
        {
            get
            {
                return containerHelper;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                containerHelper = value;
                
                this.RaisePropertyChanged(() => this.Properties);
            }
        }

        public static readonly DependencyProperty WillRefreshPropertyGridProperty =
            DependencyProperty.Register("WillRefreshPropertyGrid", typeof(bool), typeof(PropertyItemBase), new UIPropertyMetadata(false));
        public bool WillRefreshPropertyGrid
        {
            get
            {
                return (bool)GetValue(WillRefreshPropertyGridProperty);
            }
            set
            {
                SetValue(WillRefreshPropertyGridProperty, value);
            }
        }

        internal static readonly RoutedEvent ItemSelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "ItemSelectionChangedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PropertyItemBase));
        private void RaiseItemSelectionChangedEvent()
        {
            RaiseEvent(new RoutedEventArgs(PropertyItemBase.ItemSelectionChangedEvent));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        internal void RaisePropertyChanged<TMember>(Expression<Func<TMember>> propertyExpression)
        {
            this.Notify(this.PropertyChanged, propertyExpression);
        }

        internal void RaisePropertyChanged(string name)
        {
            this.Notify(this.PropertyChanged, name);
        }

        static PropertyItemBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyItemBase), new FrameworkPropertyMetadata(typeof(PropertyItemBase)));
        }

        internal PropertyItemBase()
        {
            this.GotFocus += new RoutedEventHandler(PropertyItemBase_GotFocus);
            this.RequestBringIntoView += this.PropertyItemBase_RequestBringIntoView;
            AddHandler(PropertyItemsControl.PreparePropertyItemEvent, new PropertyItemEventHandler(OnPreparePropertyItemInternal));
            AddHandler(PropertyItemsControl.ClearPropertyItemEvent, new PropertyItemEventHandler(OnClearPropertyItemInternal));
        }

        private void OnPreparePropertyItemInternal(object sender, PropertyItemEventArgs args)
        {
            args.PropertyItem.Level = this.Level + 1;
            containerHelper.PrepareChildrenPropertyItem(args.PropertyItem, args.Item);

            args.Handled = true;
        }

        private void OnClearPropertyItemInternal(object sender, PropertyItemEventArgs args)
        {
            containerHelper.ClearChildrenPropertyItem(args.PropertyItem, args.Item);
            args.PropertyItem.Level = 0;
            args.Handled = true;
        }

        private void PropertyItemBase_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        protected virtual Type GetPropertyItemType()
        {
            return null;
        }

        protected virtual string GetPropertyItemName()
        {
            return this.DisplayName;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            containerHelper.ChildrenItemsControl = GetTemplateChild(PropertyGrid.PART_PropertyItemsControl) as PropertyItemsControl;
            valueContainer = GetTemplateChild(PropertyItemBase.PART_ValueContainer) as ContentControl;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            IsSelected = true;
            if (!this.IsKeyboardFocusWithin)
            {
                this.Focus();
            }
            
            e.Handled = true;
        }

        private void PropertyItemBase_GotFocus(object sender, RoutedEventArgs e)
        {
            IsSelected = true;
            e.Handled = true;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            if (ReflectionHelper.IsPublicInstanceProperty(GetType(), e.Property.Name) && this.IsLoaded && (parentNode != null) && !parentNode.ContainerHelper.IsCleaning)
            {
                this.RaisePropertyChanged(e.Property.Name);
            }
        }

        private PropertyDefinitionCollection GetPropertItemPropertyDefinitions()
        {
            if ((this.ParentNode != null) && (this.ParentNode.PropertyDefinitions != null))
            {
                var name = this.GetPropertyItemName();
                foreach (var pd in this.ParentNode.PropertyDefinitions)
                {
                    if (pd.TargetProperties.Contains(name))
                    {
                        return pd.PropertyDefinitions;
                    }
                    else
                    {
                        var type = this.GetPropertyItemType();
                        if (type != null)
                        {
                            foreach (var targetProperty in pd.TargetProperties)
                            {
                                var targetPropertyType = targetProperty as Type;
                                
                                if ((targetPropertyType != null) && targetPropertyType.IsAssignableFrom(type))
                                {
                                    return pd.PropertyDefinitions;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        Style IPropertyContainer.PropertyContainerStyle
        {
            get { return this.PropertyContainerStyle; }
        }

        EditorDefinitionCollection IPropertyContainer.EditorDefinitions
        {
            get
            {
                return (this.ParentNode != null) ? this.ParentNode.EditorDefinitions : null;
            }
        }

        PropertyDefinitionCollection IPropertyContainer.PropertyDefinitions
        {
            get
            {
                return this.GetPropertItemPropertyDefinitions();
            }
        }

        ContainerHelper IPropertyContainer.ContainerHelper
        {
            get
            {
                return this.ContainerHelper;
            }
        }

        bool IPropertyContainer.IsCategorized
        {
            get
            {
                return isPropertyGridCategorized;
            }
        }

        bool IPropertyContainer.IsSortedAlphabetically
        {
            get
            {
                return isSortedAlphabetically;
            }
        }

        bool IPropertyContainer.AutoGenerateProperties
        {
            get
            {
                if (this.ParentNode != null)
                {
                    var propertyItemPropertyDefinitions = this.GetPropertItemPropertyDefinitions();
                    
                    if ((propertyItemPropertyDefinitions == null) || (propertyItemPropertyDefinitions.Count == 0))
                    {
                        return true;
                    }
                    
                    return this.ParentNode.AutoGenerateProperties;
                }
                return true;
            }
        }

        bool IPropertyContainer.HideInheritedProperties
        {
            get
            {
                return false;
            }
        }

        FilterInfo IPropertyContainer.FilterInfo
        {
            get { return new FilterInfo(); }
        }

        bool? IPropertyContainer.IsPropertyVisible(PropertyDescriptor pd)
        {
            if (parentNode != null)
            {
                return parentNode.IsPropertyVisible(pd);
            }

            return null;
        }
    }
}
