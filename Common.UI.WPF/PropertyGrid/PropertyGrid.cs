using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Common.UI.WPF.Core.Utilities;
using Common.UI.WPF.PropertyGrid.Commands;

namespace Common.UI.WPF.PropertyGrid
{
    [TemplatePart(Name = PART_DragThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_PropertyItemsControl, Type = typeof(PropertyItemsControl))]
    [StyleTypedProperty(Property = "PropertyContainerStyle", StyleTargetType = typeof(PropertyItemBase))]
    public class PropertyGrid : Control, ISupportInitialize, IPropertyContainer, INotifyPropertyChanged
    {
        private const string PART_DragThumb = "PART_DragThumb";
        internal const string PART_PropertyItemsControl = "PART_PropertyItemsControl";
        private static readonly ComponentResourceKey SelectedObjectAdvancedOptionsMenuKey = new ComponentResourceKey(typeof(PropertyGrid), "SelectedObjectAdvancedOptionsMenu");

        private Thumb dragThumb;
        private bool hasPendingSelectedObjectChanged;
        private int initializationCount;
        private ContainerHelper containerHelper;
        private WeakEventListener<NotifyCollectionChangedEventArgs> propertyDefinitionsListener;
        private WeakEventListener<NotifyCollectionChangedEventArgs> editorDefinitionsListener;

        public static readonly DependencyProperty AdvancedOptionsMenuProperty = DependencyProperty.Register("AdvancedOptionsMenu", typeof(ContextMenu), typeof(PropertyGrid), new UIPropertyMetadata(null));
        public ContextMenu AdvancedOptionsMenu
        {
            get
            {
                return (ContextMenu)GetValue(AdvancedOptionsMenuProperty);
            }
            set
            {
                SetValue(AdvancedOptionsMenuProperty, value);
            }
        }

        public static readonly DependencyProperty AutoGeneratePropertiesProperty = DependencyProperty.Register("AutoGenerateProperties", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
        public bool AutoGenerateProperties
        {
            get
            {
                return (bool)GetValue(AutoGeneratePropertiesProperty);
            }
            set
            {
                SetValue(AutoGeneratePropertiesProperty, value);
            }
        }

        public static readonly DependencyProperty CategoryGroupHeaderTemplateProperty = DependencyProperty.Register("CategoryGroupHeaderTemplate", typeof(DataTemplate), typeof(PropertyGrid));
        public DataTemplate CategoryGroupHeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(CategoryGroupHeaderTemplateProperty);
            }
            set
            {
                SetValue(CategoryGroupHeaderTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty ShowDescriptionByTooltipProperty = DependencyProperty.Register("ShowDescriptionByTooltip", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false));
        public bool ShowDescriptionByTooltip
        {
            get
            {
                return (bool)GetValue(ShowDescriptionByTooltipProperty);
            }
            set
            {
                SetValue(ShowDescriptionByTooltipProperty, value);
            }
        }

        public static readonly DependencyProperty ShowSummaryProperty = DependencyProperty.Register("ShowSummary", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
        public bool ShowSummary
        {
            get
            {
                return (bool)GetValue(ShowSummaryProperty);
            }
            set
            {
                SetValue(ShowSummaryProperty, value);
            }
        }

        public static readonly DependencyProperty EditorDefinitionsProperty = DependencyProperty.Register("EditorDefinitions", typeof(EditorDefinitionCollection), typeof(PropertyGrid), new UIPropertyMetadata(null, OnEditorDefinitionsChanged));
        public EditorDefinitionCollection EditorDefinitions
        {
            get
            {
                return (EditorDefinitionCollection)GetValue(EditorDefinitionsProperty);
            }
            set
            {
                SetValue(EditorDefinitionsProperty, value);
            }
        }

        private static void OnEditorDefinitionsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyGrid)
            {
                propertyGrid.OnEditorDefinitionsChanged((EditorDefinitionCollection)e.OldValue, (EditorDefinitionCollection)e.NewValue);
            }
        }

        protected virtual void OnEditorDefinitionsChanged(EditorDefinitionCollection oldValue, EditorDefinitionCollection newValue)
        {
            if (oldValue != null)
            {
                CollectionChangedEventManager.RemoveListener(oldValue, editorDefinitionsListener);
            }

            if (newValue != null)
            {
                CollectionChangedEventManager.AddListener(newValue, editorDefinitionsListener);
            }

            this.Notify(this.PropertyChanged, () => this.EditorDefinitions);
        }

        private void OnEditorDefinitionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (containerHelper != null)
            {
                containerHelper.NotifyEditorDefinitionsCollectionChanged();
            }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(PropertyGrid), new UIPropertyMetadata(null, OnFilterChanged));
        public string Filter
        {
            get
            {
                return (string)GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }

        private static void OnFilterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyGrid)
            {
                propertyGrid.OnFilterChanged((string)e.OldValue, (string)e.NewValue);
            }
        }

        protected virtual void OnFilterChanged(string oldValue, string newValue)
        {
            this.Notify(this.PropertyChanged, () => ((IPropertyContainer)this).FilterInfo);
        }

        public static readonly DependencyProperty FilterWatermarkProperty = DependencyProperty.Register("FilterWatermark", typeof(string), typeof(PropertyGrid), new UIPropertyMetadata("Search"));
        public string FilterWatermark
        {
            get
            {
                return (string)GetValue(FilterWatermarkProperty);
            }
            set
            {
                SetValue(FilterWatermarkProperty, value);
            }
        }

        public static readonly DependencyProperty HideInheritedPropertiesProperty = DependencyProperty.Register("HideInheritedProperties", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false));
        public bool HideInheritedProperties
        {
            get
            {
                return (bool)GetValue(HideInheritedPropertiesProperty);
            }
            set
            {
                SetValue(HideInheritedPropertiesProperty, value);
            }
        }

        public static readonly DependencyProperty IsCategorizedProperty = DependencyProperty.Register("IsCategorized", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true, OnIsCategorizedChanged));
        public bool IsCategorized
        {
            get
            {
                return (bool)GetValue(IsCategorizedProperty);
            }
            set
            {
                SetValue(IsCategorizedProperty, value);
            }
        }

        private static void OnIsCategorizedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyGrid)
            {
                propertyGrid.OnIsCategorizedChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsCategorizedChanged(bool oldValue, bool newValue)
        {
            this.UpdateThumb();
        }

        public static readonly DependencyProperty IsMiscCategoryLabelHiddenProperty = DependencyProperty.Register("IsMiscCategoryLabelHidden", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false));
        public bool IsMiscCategoryLabelHidden
        {
            get
            {
                return (bool)GetValue(IsMiscCategoryLabelHiddenProperty);
            }
            set
            {
                SetValue(IsMiscCategoryLabelHiddenProperty, value);
            }
        }

        public static readonly DependencyProperty IsScrollingToTopAfterRefreshProperty = DependencyProperty.Register("IsScrollingToTopAfterRefresh", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
        public bool IsScrollingToTopAfterRefresh
        {
            get
            {
                return (bool)GetValue(IsScrollingToTopAfterRefreshProperty);
            }
            set
            {
                SetValue(IsScrollingToTopAfterRefreshProperty, value);
            }
        }

        public static readonly DependencyProperty IsVirtualizingProperty = DependencyProperty.Register("IsVirtualizing", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false, OnIsVirtualizingChanged));
        public bool IsVirtualizing
        {
            get
            {
                return (bool)GetValue(IsVirtualizingProperty);
            }
            set
            {
                SetValue(IsVirtualizingProperty, value);
            }
        }

        private static void OnIsVirtualizingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyGrid)
            {
                propertyGrid.OnIsVirtualizingChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsVirtualizingChanged(bool oldValue, bool newValue)
        {
            this.UpdateContainerHelper();
        }

        public static readonly DependencyProperty NameColumnWidthProperty = DependencyProperty.Register("NameColumnWidth", typeof(double), typeof(PropertyGrid), new UIPropertyMetadata(150.0, OnNameColumnWidthChanged));
        public double NameColumnWidth
        {
            get
            {
                return (double)GetValue(NameColumnWidthProperty);
            }
            set
            {
                SetValue(NameColumnWidthProperty, value);
            }
        }

        private static void OnNameColumnWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyGrid)
            {
                propertyGrid.OnNameColumnWidthChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        protected virtual void OnNameColumnWidthChanged(double oldValue, double newValue)
        {
            if (dragThumb != null)
            {
                ((TranslateTransform)dragThumb.RenderTransform).X = newValue;
            }
        }

        public static readonly DependencyProperty PropertyNameLeftPaddingProperty = DependencyProperty.Register("PropertyNameLeftPadding", typeof(double), typeof(PropertyGrid), new UIPropertyMetadata(15.0));
        public double PropertyNameLeftPadding
        {
            get
            {
                return (double)GetValue(PropertyNameLeftPaddingProperty);
            }
            set
            {
                SetValue(PropertyNameLeftPaddingProperty, value);
            }
        }

        public static readonly DependencyProperty PropertyNameTextWrappingProperty = DependencyProperty.Register("PropertyNameTextWrapping", typeof(TextWrapping), typeof(PropertyGrid), new UIPropertyMetadata(TextWrapping.NoWrap));
        public TextWrapping PropertyNameTextWrapping
        {
            get
            {
                return (TextWrapping)GetValue(PropertyNameTextWrappingProperty);
            }
            set
            {
                SetValue(PropertyNameTextWrappingProperty, value);
            }
        }

        public IList Properties
        {
            get
            {
                return (containerHelper != null) ? containerHelper.Properties : null;
            }
        }
        
        public static readonly DependencyProperty PropertyContainerStyleProperty =
            DependencyProperty.Register("PropertyContainerStyle", typeof(Style), typeof(PropertyGrid), new UIPropertyMetadata(null, OnPropertyContainerStyleChanged));
        
        public Style PropertyContainerStyle
        {
            get { return (Style)GetValue(PropertyContainerStyleProperty); }
            set { SetValue(PropertyContainerStyleProperty, value); }
        }

        private static void OnPropertyContainerStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid owner)
            {
                owner.OnPropertyContainerStyleChanged((Style)e.OldValue, (Style)e.NewValue);
            }
        }

        protected virtual void OnPropertyContainerStyleChanged(Style oldValue, Style newValue)
        {
        }

        public static readonly DependencyProperty PropertyDefinitionsProperty =
            DependencyProperty.Register("PropertyDefinitions", typeof(PropertyDefinitionCollection), typeof(PropertyGrid), new UIPropertyMetadata(null, OnPropertyDefinitionsChanged));

        public PropertyDefinitionCollection PropertyDefinitions
        {
            get
            {
                return (PropertyDefinitionCollection)GetValue(PropertyDefinitionsProperty);
            }
            set
            {
                SetValue(PropertyDefinitionsProperty, value);
            }
        }

        private static void OnPropertyDefinitionsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid owner)
            {
                owner.OnPropertyDefinitionsChanged((PropertyDefinitionCollection)e.OldValue, (PropertyDefinitionCollection)e.NewValue);
            }
        }

        protected virtual void OnPropertyDefinitionsChanged(PropertyDefinitionCollection oldValue, PropertyDefinitionCollection newValue)
        {
            if (oldValue != null)
            {
                CollectionChangedEventManager.RemoveListener(oldValue, propertyDefinitionsListener);
            }

            if (newValue != null)
            {
                CollectionChangedEventManager.AddListener(newValue, propertyDefinitionsListener);
            }

            this.Notify(this.PropertyChanged, () => this.PropertyDefinitions);
        }

        private void OnPropertyDefinitionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (containerHelper != null)
            {
                containerHelper.NotifyPropertyDefinitionsCollectionChanged();
            }

            if (this.IsLoaded)
            {
                this.UpdateContainerHelper();
            }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false, OnIsReadOnlyChanged));
        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        private static void OnIsReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyGrid)
            {
                propertyGrid.OnIsReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue)
        {
            this.UpdateContainerHelper();
        }

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGrid), new UIPropertyMetadata(null, OnSelectedObjectChanged));
        public object SelectedObject
        {
            get
            {
                return (object)GetValue(SelectedObjectProperty);
            }
            set
            {
                SetValue(SelectedObjectProperty, value);
            }
        }

        private static void OnSelectedObjectChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyInspector)
            {
                propertyInspector.OnSelectedObjectChanged((object)e.OldValue, (object)e.NewValue);
            }
        }

        protected virtual void OnSelectedObjectChanged(object oldValue, object newValue)
        {
            if (initializationCount != 0)
            {
                hasPendingSelectedObjectChanged = true;
                return;
            }

            this.UpdateContainerHelper();

            RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, PropertyGrid.SelectedObjectChangedEvent));
        }

        public static readonly DependencyProperty SelectedObjectTypeProperty = DependencyProperty.Register("SelectedObjectType", typeof(Type), typeof(PropertyGrid), new UIPropertyMetadata(null, OnSelectedObjectTypeChanged));
        public Type SelectedObjectType
        {
            get
            {
                return (Type)GetValue(SelectedObjectTypeProperty);
            }
            set
            {
                SetValue(SelectedObjectTypeProperty, value);
            }
        }

        private static void OnSelectedObjectTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyGrid)
            {
                propertyGrid.OnSelectedObjectTypeChanged((Type)e.OldValue, (Type)e.NewValue);
            }
        }

        protected virtual void OnSelectedObjectTypeChanged(Type oldValue, Type newValue)
        {
        }

        public static readonly DependencyProperty SelectedObjectTypeNameProperty = DependencyProperty.Register("SelectedObjectTypeName", typeof(string), typeof(PropertyGrid), new UIPropertyMetadata(string.Empty));
        public string SelectedObjectTypeName
        {
            get
            {
                return (string)GetValue(SelectedObjectTypeNameProperty);
            }
            set
            {
                SetValue(SelectedObjectTypeNameProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedObjectNameProperty = DependencyProperty.Register("SelectedObjectName", typeof(string), typeof(PropertyGrid), new UIPropertyMetadata(string.Empty, OnSelectedObjectNameChanged, OnCoerceSelectedObjectName));
        public string SelectedObjectName
        {
            get
            {
                return (string)GetValue(SelectedObjectNameProperty);
            }
            set
            {
                SetValue(SelectedObjectNameProperty, value);
            }
        }

        private static object OnCoerceSelectedObjectName(DependencyObject o, object baseValue)
        {
            if (o is PropertyGrid propertyGrid)
            {
                if ((propertyGrid.SelectedObject is FrameworkElement) && (String.IsNullOrEmpty((String)baseValue)))
                {
                    return "<no name>";
                }
            }

            return baseValue;
        }

        private static void OnSelectedObjectNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyGrid)
            {
                propertyGrid.SelectedObjectNameChanged((string)e.OldValue, (string)e.NewValue);
            }
        }

        protected virtual void SelectedObjectNameChanged(string oldValue, string newValue)
        {
        }

        private static readonly DependencyPropertyKey SelectedPropertyItemPropertyKey = DependencyProperty.RegisterReadOnly("SelectedPropertyItem", typeof(PropertyItemBase), typeof(PropertyGrid), new UIPropertyMetadata(null, OnSelectedPropertyItemChanged));
        public static readonly DependencyProperty SelectedPropertyItemProperty = SelectedPropertyItemPropertyKey.DependencyProperty;
        public PropertyItemBase SelectedPropertyItem
        {
            get
            {
                return (PropertyItemBase)GetValue(SelectedPropertyItemProperty);
            }
            internal set
            {
                SetValue(SelectedPropertyItemPropertyKey, value);
            }
        }

        private static void OnSelectedPropertyItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is PropertyGrid propertyGrid)
            {
                propertyGrid.OnSelectedPropertyItemChanged((PropertyItemBase)e.OldValue, (PropertyItemBase)e.NewValue);
            }
        }

        protected virtual void OnSelectedPropertyItemChanged(PropertyItemBase oldValue, PropertyItemBase newValue)
        {
            if (oldValue != null)
            {
                oldValue.IsSelected = false;
            }

            if (newValue != null)
            {
                newValue.IsSelected = true;
            }

            this.SelectedProperty = ((newValue != null) && (containerHelper != null)) ? containerHelper.ItemFromContainer(newValue) : null;

            RaiseEvent(new RoutedPropertyChangedEventArgs<PropertyItemBase>(oldValue, newValue, PropertyGrid.SelectedPropertyItemChangedEvent));
        }
        
        public static readonly DependencyProperty SelectedPropertyProperty =
            DependencyProperty.Register("SelectedProperty", typeof(object), typeof(PropertyGrid), new UIPropertyMetadata(null, OnSelectedPropertyChanged));
        
        public object SelectedProperty
        {
            get { return (object)GetValue(SelectedPropertyProperty); }
            set { SetValue(SelectedPropertyProperty, value); }
        }

        private static void OnSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            PropertyGrid propertyGrid = sender as PropertyGrid;
            if (propertyGrid != null)
            {
                propertyGrid.OnSelectedPropertyChanged((object)args.OldValue, (object)args.NewValue);
            }
        }

        private void OnSelectedPropertyChanged(object oldValue, object newValue)
        {
            if (containerHelper != null)
            {
                object currentSelectedProperty = containerHelper.ItemFromContainer(this.SelectedPropertyItem);
                if (!object.Equals(currentSelectedProperty, newValue))
                {
                    this.SelectedPropertyItem = containerHelper.ContainerFromItem(newValue);
                }
            }
        }

        public static readonly DependencyProperty ShowAdvancedOptionsProperty = DependencyProperty.Register("ShowAdvancedOptions", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false));
        public bool ShowAdvancedOptions
        {
            get
            {
                return (bool)GetValue(ShowAdvancedOptionsProperty);
            }
            set
            {
                SetValue(ShowAdvancedOptionsProperty, value);
            }
        }

        public static readonly DependencyProperty ShowHorizontalScrollBarProperty = DependencyProperty.Register("ShowHorizontalScrollBar", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false));
        public bool ShowHorizontalScrollBar
        {
            get
            {
                return (bool)GetValue(ShowHorizontalScrollBarProperty);
            }
            set
            {
                SetValue(ShowHorizontalScrollBarProperty, value);
            }
        }

        public static readonly DependencyProperty ShowPreviewProperty = DependencyProperty.Register("ShowPreview", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(false));
        public bool ShowPreview
        {
            get
            {
                return (bool)GetValue(ShowPreviewProperty);
            }
            set
            {
                SetValue(ShowPreviewProperty, value);
            }
        }

        public static readonly DependencyProperty ShowSearchBoxProperty = DependencyProperty.Register("ShowSearchBox", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
        public bool ShowSearchBox
        {
            get
            {
                return (bool)GetValue(ShowSearchBoxProperty);
            }
            set
            {
                SetValue(ShowSearchBoxProperty, value);
            }
        }

        public static readonly DependencyProperty ShowSortOptionsProperty = DependencyProperty.Register("ShowSortOptions", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
        public bool ShowSortOptions
        {
            get
            {
                return (bool)GetValue(ShowSortOptionsProperty);
            }
            set
            {
                SetValue(ShowSortOptionsProperty, value);
            }
        }

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register("ShowTitle", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
        public bool ShowTitle
        {
            get
            {
                return (bool)GetValue(ShowTitleProperty);
            }
            set
            {
                SetValue(ShowTitleProperty, value);
            }
        }

        public static readonly DependencyProperty UpdateTextBoxSourceOnEnterKeyProperty = DependencyProperty.Register("UpdateTextBoxSourceOnEnterKey", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));
        public bool UpdateTextBoxSourceOnEnterKey
        {
            get
            {
                return (bool)GetValue(UpdateTextBoxSourceOnEnterKeyProperty);
            }
            set
            {
                SetValue(UpdateTextBoxSourceOnEnterKeyProperty, value);
            }
        }

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public PropertyGrid()
        {
            propertyDefinitionsListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(this.OnPropertyDefinitionsCollectionChanged);
            editorDefinitionsListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(this.OnEditorDefinitionsCollectionChanged);
            
            UpdateContainerHelper();

            this.SetCurrentValue(PropertyGrid.EditorDefinitionsProperty, new EditorDefinitionCollection());
            
            PropertyDefinitions = new PropertyDefinitionCollection();
            this.PropertyValueChanged += this.PropertyGrid_PropertyValueChanged;

            AddHandler(PropertyItemBase.ItemSelectionChangedEvent, new RoutedEventHandler(OnItemSelectionChanged));
            AddHandler(PropertyItemsControl.PreparePropertyItemEvent, new PropertyItemEventHandler(OnPreparePropertyItemInternal));
            AddHandler(PropertyItemsControl.ClearPropertyItemEvent, new PropertyItemEventHandler(OnClearPropertyItemInternal));
            CommandBindings.Add(new CommandBinding(PropertyGridCommands.ClearFilter, ClearFilter, CanClearFilter));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (dragThumb != null)
            { 
                dragThumb.DragDelta -= DragThumb_DragDelta;
            }

            dragThumb = GetTemplateChild(PART_DragThumb) as Thumb;

            if (dragThumb != null)
            { 
                dragThumb.DragDelta += DragThumb_DragDelta;
            }

            if (containerHelper != null)
            {
                containerHelper.ChildrenItemsControl = GetTemplateChild(PART_PropertyItemsControl) as PropertyItemsControl;
            }
            
            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = NameColumnWidth;
            if (dragThumb != null)
            {
                dragThumb.RenderTransform = moveTransform;
            }

            this.UpdateThumb();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if ((this.SelectedPropertyItem != null) && (e.Key == Key.Enter) && this.UpdateTextBoxSourceOnEnterKey && (e.OriginalSource is TextBox textBox) && !textBox.AcceptsReturn)
            {
                BindingExpression be = textBox.GetBindingExpression(TextBox.TextProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            if (ReflectionHelper.IsPublicInstanceProperty(GetType(), e.Property.Name))
            {
                this.Notify(this.PropertyChanged, e.Property.Name);
            }
        }

        private void OnItemSelectionChanged(object sender, RoutedEventArgs args)
        {
            PropertyItemBase item = (PropertyItemBase)args.OriginalSource;
            if (item.IsSelected)
            {
                SelectedPropertyItem = item;
            }
            else
            {
                if (object.ReferenceEquals(item, SelectedPropertyItem))
                {
                    SelectedPropertyItem = null;
                }
            }
        }

        private void OnPreparePropertyItemInternal(object sender, PropertyItemEventArgs args)
        {
            if (containerHelper != null)
            {
                containerHelper.PrepareChildrenPropertyItem(args.PropertyItem, args.Item);
            }
            args.Handled = true;
        }

        private void OnClearPropertyItemInternal(object sender, PropertyItemEventArgs args)
        {
            if (containerHelper != null)
            {
                containerHelper.ClearChildrenPropertyItem(args.PropertyItem, args.Item);
            }
            args.Handled = true;
        }

        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            NameColumnWidth = Math.Min(Math.Max(this.ActualWidth * 0.1, NameColumnWidth + e.HorizontalChange), this.ActualWidth * 0.9);
        }

        private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (e.OriginalSource is PropertyItem modifiedPropertyItem)
            {
                if (modifiedPropertyItem.WillRefreshPropertyGrid)
                {
                    this.UpdateContainerHelper();
                }

                if ((modifiedPropertyItem.ParentNode is PropertyItem parentPropertyItem) && parentPropertyItem.IsExpandable)
                {
                    this.RebuildPropertyItemEditor(parentPropertyItem);
                }
            }
        }

        private void ClearFilter(object sender, ExecutedRoutedEventArgs e)
        {
            Filter = String.Empty;
        }

        private void CanClearFilter(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrEmpty(Filter);
        }
        
        public double GetScrollPosition()
        {
            var scrollViewer = this.GetScrollViewer();
            if (scrollViewer != null)
            {
                return scrollViewer.VerticalOffset;
            }
            return 0d;
        }

        public void ScrollToPosition(double position)
        {
            var scrollViewer = this.GetScrollViewer();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(position);
            }
        }

        public void ScrollToTop()
        {
            var scrollViewer = this.GetScrollViewer();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToTop();
            }
        }

        public void ScrollToBottom()
        {
            var scrollViewer = this.GetScrollViewer();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToBottom();
            }
        }

        public void CollapseAllProperties()
        {
            if (containerHelper != null)
            {
                containerHelper.SetPropertiesExpansion(false);
            }
        }

        public void ExpandAllProperties()
        {
            if (containerHelper != null)
            {
                containerHelper.SetPropertiesExpansion(true);
            }
        }

        public void ExpandProperty(string propertyName)
        {
            if (containerHelper != null)
            {
                containerHelper.SetPropertiesExpansion(propertyName, true);
            }
        }

        public void CollapseProperty(string propertyName)
        {
            if (containerHelper != null)
            {
                containerHelper.SetPropertiesExpansion(propertyName, false);
            }
        }

        private ScrollViewer GetScrollViewer()
        {
            if ((containerHelper != null) && (containerHelper.ChildrenItemsControl != null))
            {
                return TreeHelper.FindChild<ScrollViewer>(containerHelper.ChildrenItemsControl);
            }
            return null;
        }

        private void RebuildPropertyItemEditor(PropertyItem propertyItem)
        {
            if (propertyItem != null)
            {
                propertyItem.RebuildEditor();
            }
        }

        private void UpdateContainerHelper()
        {
            ItemsControl childrenItemsControl = containerHelper?.ChildrenItemsControl;
            ObjectContainerHelperBase objectContainerHelper = null;
            
            objectContainerHelper = new ObjectContainerHelper(this, SelectedObject);
            objectContainerHelper.ObjectsGenerated += this.ObjectContainerHelper_ObjectsGenerated;
            objectContainerHelper.GenerateProperties();
        }

        private void SetContainerHelper(ContainerHelper containerHelper)
        {
            if (this.containerHelper != null)
            {
                this.containerHelper.ClearHelper();
            }
            this.containerHelper = containerHelper;
        }

        private void FinalizeUpdateContainerHelper(ItemsControl childrenItemsControl)
        {
            if (containerHelper != null)
            {
                containerHelper.ChildrenItemsControl = childrenItemsControl;
            }

            if (this.IsScrollingToTopAfterRefresh)
            {
                this.ScrollToTop();
            }
            
            this.Notify(this.PropertyChanged, () => this.Properties);
        }

        private void UpdateThumb()
        {
            if (dragThumb != null)
            {
                if (IsCategorized)
                {
                    dragThumb.Margin = new Thickness(6, 0, 0, 0);
                }
                else
                {
                    dragThumb.Margin = new Thickness(-1, 0, 0, 0);
                }
            }
        }

        protected virtual Predicate<object> CreateFilter(string filter)
        {
            return null;
        }
        
        public void Update()
        {
            if (containerHelper != null)
            {
                containerHelper.UpdateValuesFromSource();
            }
        }

        private void ObjectContainerHelper_ObjectsGenerated(object sender, EventArgs e)
        {
            if (sender is ObjectContainerHelperBase objectContainerHelper)
            {
                objectContainerHelper.ObjectsGenerated -= this.ObjectContainerHelper_ObjectsGenerated;
                this.SetContainerHelper(objectContainerHelper);
                this.FinalizeUpdateContainerHelper(objectContainerHelper.ChildrenItemsControl);

                RaiseEvent(new RoutedEventArgs(PropertyGrid.PropertiesGeneratedEvent, this));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public static readonly RoutedEvent PropertyValueChangedEvent = EventManager.RegisterRoutedEvent("PropertyValueChanged", RoutingStrategy.Bubble, typeof(PropertyValueChangedEventHandler), typeof(PropertyGrid));
        public event PropertyValueChangedEventHandler PropertyValueChanged
        {
            add
            {
                AddHandler(PropertyValueChangedEvent, value);
            }
            remove
            {
                RemoveHandler(PropertyValueChangedEvent, value);
            }
        }
        
        public static readonly RoutedEvent SelectedPropertyItemChangedEvent = EventManager.RegisterRoutedEvent("SelectedPropertyItemChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<PropertyItemBase>), typeof(PropertyGrid));
        public event RoutedPropertyChangedEventHandler<PropertyItemBase> SelectedPropertyItemChanged
        {
            add
            {
                AddHandler(SelectedPropertyItemChangedEvent, value);
            }
            remove
            {
                RemoveHandler(SelectedPropertyItemChangedEvent, value);
            }
        }

        public static readonly RoutedEvent SelectedObjectChangedEvent = EventManager.RegisterRoutedEvent("SelectedObjectChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(PropertyGrid));
        public event RoutedPropertyChangedEventHandler<object> SelectedObjectChanged
        {
            add
            {
                AddHandler(SelectedObjectChangedEvent, value);
            }
            remove
            {
                RemoveHandler(SelectedObjectChangedEvent, value);
            }
        }

        public event IsPropertyBrowsableHandler IsPropertyBrowsable;

        public static readonly RoutedEvent PreparePropertyItemEvent = EventManager.RegisterRoutedEvent("PreparePropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyGrid));
        public event PropertyItemEventHandler PreparePropertyItem
        {
            add
            {
                AddHandler(PropertyGrid.PreparePropertyItemEvent, value);
            }
            remove
            {
                RemoveHandler(PropertyGrid.PreparePropertyItemEvent, value);
            }
        }
        
        public static void AddPreparePropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
        {
            element.AddHandler(PropertyGrid.PreparePropertyItemEvent, handler);
        }
        
        public static void RemovePreparePropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
        {
            element.RemoveHandler(PropertyGrid.PreparePropertyItemEvent, handler);
        }

        internal static void RaisePreparePropertyItemEvent(UIElement source, PropertyItemBase propertyItem, object item)
        {
            source.RaiseEvent(new PropertyItemEventArgs(PropertyGrid.PreparePropertyItemEvent, source, propertyItem, item));
        }

        public static readonly RoutedEvent ClearPropertyItemEvent = EventManager.RegisterRoutedEvent("ClearPropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyGrid));
        public event PropertyItemEventHandler ClearPropertyItem
        {
            add
            {
                AddHandler(PropertyGrid.ClearPropertyItemEvent, value);
            }
            remove
            {
                RemoveHandler(PropertyGrid.ClearPropertyItemEvent, value);
            }
        }
        
        public static readonly RoutedEvent PropertiesGeneratedEvent = EventManager.RegisterRoutedEvent("PropertiesGenerated", RoutingStrategy.Bubble, typeof(EventHandler), typeof(PropertyGrid));
        public event RoutedEventHandler PropertiesGenerated
        {
            add
            {
                AddHandler(PropertiesGeneratedEvent, value);
            }
            remove
            {
                RemoveHandler(PropertiesGeneratedEvent, value);
            }
        }
        
        public static void AddClearPropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
        {
            element.AddHandler(PropertyGrid.ClearPropertyItemEvent, handler);
        }
        
        public static void RemoveClearPropertyItemHandler(UIElement element, PropertyItemEventHandler handler)
        {
            element.RemoveHandler(PropertyGrid.ClearPropertyItemEvent, handler);
        }

        internal static void RaiseClearPropertyItemEvent(UIElement source, PropertyItemBase propertyItem, object item)
        {
            source.RaiseEvent(new PropertyItemEventArgs(PropertyGrid.ClearPropertyItemEvent, source, propertyItem, item));
        }

        public override void BeginInit()
        {
            base.BeginInit();
            initializationCount++;
        }

        public override void EndInit()
        {
            base.EndInit();
            if (--initializationCount == 0)
            {
                if (hasPendingSelectedObjectChanged)
                {
                    this.UpdateContainerHelper();
                    hasPendingSelectedObjectChanged = false;
                }
                if (containerHelper != null)
                {
                    containerHelper.OnEndInit();
                }
            }
        }

        FilterInfo IPropertyContainer.FilterInfo
        {
            get
            {
                return new FilterInfo()
                {
                    Predicate = this.CreateFilter(this.Filter),
                    InputString = this.Filter
                };
            }
        }

        ContainerHelper IPropertyContainer.ContainerHelper
        {
            get
            {
                return containerHelper;
            }
        }

        bool IPropertyContainer.IsSortedAlphabetically
        {
            get
            {
                return true;
            }
        }


        bool? IPropertyContainer.IsPropertyVisible(PropertyDescriptor pd)
        {
            var handler = this.IsPropertyBrowsable;
            
            if (handler != null)
            {
                var isBrowsableArgs = new IsPropertyBrowsableArgs(pd);
                handler(this, isBrowsableArgs);

                return isBrowsableArgs.IsBrowsable;
            }

            return null;
        }
    }
    
    public delegate void PropertyValueChangedEventHandler(object sender, PropertyValueChangedEventArgs e);
    public class PropertyValueChangedEventArgs : RoutedEventArgs
    {
        public object NewValue
        {
            get;
            set;
        }

        public object OldValue
        {
            get;
            set;
        }

        public PropertyValueChangedEventArgs(RoutedEvent routedEvent, object source, object oldValue, object newValue)
          : base(routedEvent, source)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
    
    public delegate void PropertyItemEventHandler(object sender, PropertyItemEventArgs e);
    public class PropertyItemEventArgs : RoutedEventArgs
    {
        public PropertyItemBase PropertyItem
        {
            get;
            private set;
        }

        public object Item
        {
            get;
            private set;
        }

        public PropertyItemEventArgs(RoutedEvent routedEvent, object source, PropertyItemBase propertyItem, object item)
          : base(routedEvent, source)
        {
            this.PropertyItem = propertyItem;
            this.Item = item;
        }
    }

    public class PropertyArgs : RoutedEventArgs
    {
        public PropertyArgs(PropertyDescriptor pd)
        {
            this.PropertyDescriptor = pd;
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get;
            private set;
        }
    }

    public delegate void IsPropertyBrowsableHandler(object sender, IsPropertyBrowsableArgs e);

    public class IsPropertyBrowsableArgs : PropertyArgs
    {
        public IsPropertyBrowsableArgs(PropertyDescriptor pd)
          : base(pd)
        {
        }

        public bool? IsBrowsable
        {
            get;
            set;
        }
    }
}
