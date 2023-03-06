using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Common.UI.WPF.PropertyGrid
{
    [TemplatePart(Name = "content", Type = typeof(ContentControl))]
    public class PropertyItem : CustomPropertyItem
    {
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(PropertyItem), new UIPropertyMetadata(false, OnIsReadOnlyChanged));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void OnIsReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var propertyItem = o as PropertyItem;
            if (propertyItem != null)
            {
                propertyItem.OnIsReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue)
        {
            if (this.IsLoaded)
            {
                this.RebuildEditor();
            }
        }
        
        public static readonly DependencyProperty IsInvalidProperty =
            DependencyProperty.Register("IsInvalid", typeof(bool), typeof(PropertyItem), new UIPropertyMetadata(false, OnIsInvalidChanged));

        public bool IsInvalid
        {
            get
            {
                return (bool)GetValue(IsInvalidProperty);
            }
            internal set
            {
                SetValue(IsInvalidProperty, value);
            }
        }

        private static void OnIsInvalidChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var propertyItem = o as PropertyItem;
            if (propertyItem != null)
            { 
                propertyItem.OnIsInvalidChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual void OnIsInvalidChanged(bool oldValue, bool newValue)
        {
            var be = this.GetBindingExpression(PropertyItem.ValueProperty);

            if (newValue)
            {
                var validationError = new ValidationError(new InvalidValueValidationRule(), be);
                validationError.ErrorContent = "Value could not be converted.";
                Validation.MarkInvalid(be, validationError);
            }
            else
            {
                Validation.ClearInvalid(be);
            }
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get;
            internal set;
        }

        public string PropertyName
        {
            get
            {
                return (this.DescriptorDefinition != null) ? this.DescriptorDefinition.PropertyName : null;
            }
        }

        public Type PropertyType
        {
            get
            {
                return (PropertyDescriptor != null) ? PropertyDescriptor.PropertyType : null;
            }
        }

        internal DescriptorPropertyDefinitionBase DescriptorDefinition
        {
            get;
            private set;
        }
        
        public object Instance
        {
            get;
            internal set;
        }

        protected override string GetPropertyItemName()
        {
            return this.PropertyName;
        }

        protected override Type GetPropertyItemType()
        {
            return this.PropertyType;
        }

        protected override void OnIsExpandedChanged(bool oldValue, bool newValue)
        {
            if (newValue && this.IsLoaded)
            {
                this.GenerateExpandedPropertyItems();
            }
        }

        protected override object OnCoerceValueChanged(object baseValue)
        {
            BindingExpression be = this.GetBindingExpression(PropertyItem.ValueProperty);
            this.SetRedInvalidBorder(be);
            return baseValue;
        }

        protected override void OnValueChanged(object oldValue, object newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            
            if ((newValue == null) && (this.DescriptorDefinition != null) && (this.DescriptorDefinition.DefaultValue != null))
            {
                this.SetCurrentValue(PropertyItem.ValueProperty, this.DescriptorDefinition.DefaultValue);
            }
        }

        internal void SetRedInvalidBorder(BindingExpression be)
        {
            if ((be != null) && be.DataItem is DescriptorPropertyDefinitionBase)
            {
                DescriptorPropertyDefinitionBase descriptor = be.DataItem as DescriptorPropertyDefinitionBase;
                if (Validation.GetHasError(descriptor))
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        var errors = Validation.GetErrors(descriptor);
                        Validation.MarkInvalid(be, errors[0]);
                    }));
                }
            }
        }

        internal void RebuildEditor()
        {
            var objectContainerHelperBase = this.ContainerHelper as ObjectContainerHelperBase;
            
            var editor = objectContainerHelperBase.GenerateChildrenEditorElement(this);
            if (editor != null)
            {
                ContainerHelper.SetIsGenerated(editor, true);
                this.Editor = editor;
                
                var be = this.GetBindingExpression(PropertyItem.ValueProperty);
                if (be != null)
                {
                    be.UpdateSource();
                    this.SetRedInvalidBorder(be);
                }
            }
        }
        
        private void OnDefinitionContainerHelperInvalidated(object sender, EventArgs e)
        {
            if (this.ContainerHelper != null)
            {
                this.ContainerHelper.ClearHelper();
            }
            var helper = this.DescriptorDefinition.CreateContainerHelper(this);
            this.ContainerHelper = helper;
            if (this.IsExpanded)
            {
                helper.GenerateProperties();
            }
        }

        private void Init(DescriptorPropertyDefinitionBase definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            if (this.ContainerHelper != null)
            {
                this.ContainerHelper.ClearHelper();
            }

            this.DescriptorDefinition = definition;
            this.ContainerHelper = definition.CreateContainerHelper(this);
            definition.ContainerHelperInvalidated += new EventHandler(OnDefinitionContainerHelperInvalidated);
            this.Loaded += this.PropertyItem_Loaded;
        }

        private void GenerateExpandedPropertyItems()
        {
            if (this.IsExpanded)
            {
                var objectContainerHelper = ContainerHelper as ObjectContainerHelperBase;
                if (objectContainerHelper != null)
                {
                    objectContainerHelper.GenerateProperties();
                }
            }
        }

        private void PropertyItem_Loaded(object sender, RoutedEventArgs e)
        {
            this.GenerateExpandedPropertyItems();
        }

        internal PropertyItem(DescriptorPropertyDefinitionBase definition)
          : base(definition.IsPropertyGridCategorized, !definition.PropertyType.IsArray)
        {
            this.Init(definition);
        }

        private class InvalidValueValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                return new ValidationResult(false, null);
            }
        }
    }
}
