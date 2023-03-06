using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Common.UI.WPF.PropertyGrid.Attributes;
using Common.UI.WPF.PropertyGrid.Commands;
using Common.UI.WPF.PropertyGrid.Editors;

namespace Common.UI.WPF.PropertyGrid
{
    internal abstract class DescriptorPropertyDefinitionBase : DependencyObject
    {
        private string category;
        public string Category
        {
            get
            {
                return category;
            }
            internal set
            {
                category = value;
            }
        }

        private string categoryValue;
        public string CategoryValue
        {
            get
            {
                return categoryValue;
            }
            internal set
            {
                categoryValue = value;
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            internal set
            {
                description = value;
            }
        }

        private string displayName;
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            internal set
            {
                displayName = value;
            }
        }

        private object defaultValue;
        public object DefaultValue
        {
            get
            {
                return defaultValue;
            }
            set
            {
                defaultValue = value;
            }
        }

        private int displayOrder;
        public int DisplayOrder
        {
            get
            {
                return displayOrder;
            }
            internal set
            {
                displayOrder = value;
            }
        }

        private bool expandableAttribute;
        internal bool ExpandableAttribute
        {
            get
            {
                return expandableAttribute;
            }
            set
            {
                expandableAttribute = value;
                this.UpdateIsExpandable();
            }
        }

        private bool isReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
        }

        private IList<Type> newItemTypes;
        public IList<Type> NewItemTypes
        {
            get
            {
                return newItemTypes;
            }
        }

        private IEnumerable<CommandBinding> commandBindings;
        public IEnumerable<CommandBinding> CommandBindings
        {
            get
            {
                return commandBindings;
            }
        }

        internal abstract PropertyDescriptor PropertyDescriptor
        {
            get;
        }

        public string PropertyName
        {
            get
            {
                return PropertyDescriptor.Name;
            }
        }

        public Type PropertyType
        {
            get
            {
                return PropertyDescriptor.PropertyType;
            }
        }

        internal DescriptorPropertyDefinitionBase(bool isPropertyGridCategorized)
        {
            this.IsPropertyGridCategorized = isPropertyGridCategorized;
        }

        protected virtual string ComputeCategory()
        {
            return null;
        }

        protected virtual string ComputeCategoryValue()
        {
            return null;
        }

        protected virtual string ComputeDescription()
        {
            return null;
        }

        protected virtual int ComputeDisplayOrder(bool isPropertyGridCategorized)
        {
            return int.MaxValue;
        }

        protected virtual bool ComputeExpandableAttribute()
        {
            return false;
        }

        protected virtual object ComputeDefaultValueAttribute()
        {
            return null;
        }

        protected abstract bool ComputeIsExpandable();

        protected virtual IList<Type> ComputeNewItemTypes()
        {
            return null;
        }

        protected virtual bool ComputeIsReadOnly()
        {
            return false;
        }

        protected virtual bool ComputeCanResetValue()
        {
            return false;
        }

        protected virtual object ComputeAdvancedOptionsTooltip()
        {
            return null;
        }

        protected virtual void ResetValue()
        {
            var binding = BindingOperations.GetBindingExpressionBase(this, DescriptorPropertyDefinition.ValueProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }
        }

        protected abstract BindingBase CreateValueBinding();

        internal abstract ObjectContainerHelperBase CreateContainerHelper(IPropertyContainer parent);

        internal void RaiseContainerHelperInvalidated()
        {
            if (this.ContainerHelperInvalidated != null)
            {
                this.ContainerHelperInvalidated(this, EventArgs.Empty);
            }
        }

        internal virtual ITypeEditor CreateDefaultEditor(PropertyItem propertyItem)
        {
            return null;
        }

        internal virtual ITypeEditor CreateAttributeEditor()
        {
            return null;
        }

        internal void UpdateAdvanceOptionsForItem(DependencyObject dependencyObject, DependencyPropertyDescriptor dpDescriptor, out object tooltip)
        {
            tooltip = StringConstants.Default;
            bool isResource = typeof(Style).IsAssignableFrom(PropertyType);
            bool isDynamicResource = typeof(DynamicResourceExtension).IsAssignableFrom(PropertyType);

            if (isResource)
            {
                tooltip = StringConstants.Resource;
            }
            else
            {
                if ((dependencyObject != null) && (dpDescriptor != null))
                {
                    if (BindingOperations.GetBindingExpressionBase(dependencyObject, dpDescriptor.DependencyProperty) != null)
                    {
                        tooltip = StringConstants.Databinding;
                    }
                    else
                    {
                        BaseValueSource bvs = DependencyPropertyHelper.GetValueSource(dependencyObject, dpDescriptor.DependencyProperty).BaseValueSource;

                        switch (bvs)
                        {
                            case BaseValueSource.Inherited:
                            case BaseValueSource.DefaultStyle:
                            case BaseValueSource.ImplicitStyleReference:
                                tooltip = StringConstants.Inheritance;
                                break;
                            case BaseValueSource.DefaultStyleTrigger:
                                break;
                            case BaseValueSource.Style:
                                tooltip = StringConstants.StyleSetter;
                                break;
                            case BaseValueSource.Local:
                                tooltip = StringConstants.Local;
                                break;
                        }
                    }
                }
                else
                {
                    if (!object.Equals(this.Value, this.DefaultValue))
                    {
                        if (this.DefaultValue != null)
                        {
                            tooltip = StringConstants.Local;
                        }
                        else
                        {
                            if (this.PropertyType.IsValueType)
                            {
                                var defaultValue = Activator.CreateInstance(this.PropertyType);
                                
                                if (!object.Equals(this.Value, defaultValue))
                                {
                                    tooltip = StringConstants.Local;
                                }
                            }
                            else
                            {
                                
                                if (this.Value != null)
                                {
                                    tooltip = StringConstants.Local;
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void UpdateAdvanceOptions()
        {
            this.AdvancedOptionsTooltip = this.ComputeAdvancedOptionsTooltip();
        }

        internal void UpdateIsExpandable()
        {
            this.IsExpandable = this.ComputeIsExpandable() && (this.ExpandableAttribute);
        }

        internal void UpdateValueFromSource()
        {
            var bindingExpr = BindingOperations.GetBindingExpressionBase(this, DescriptorPropertyDefinitionBase.ValueProperty);
            if (bindingExpr != null)
            {
                bindingExpr.UpdateTarget();
            }
        }

        internal object ComputeDescriptionForItem(object item)
        {
            PropertyDescriptor pd = item as PropertyDescriptor;
            
            var displayAttribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(pd);
            if (displayAttribute != null)
            {
                return displayAttribute.GetDescription();
            }

            var descriptionAtt = PropertyGridUtilities.GetAttribute<DescriptionAttribute>(pd);
            return (descriptionAtt != null) ? descriptionAtt.Description: pd.Description;
        }

        internal object ComputeNewItemTypesForItem(object item)
        {
            PropertyDescriptor pd = item as PropertyDescriptor;
            var attribute = PropertyGridUtilities.GetAttribute<NewItemTypesAttribute>(pd);

            return (attribute != null) ? attribute.Types : null;
        }

        internal object ComputeDisplayOrderForItem(object item)
        {
            PropertyDescriptor pd = item as PropertyDescriptor;
            var displayAttribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(PropertyDescriptor);
            if (displayAttribute != null)
            {
                var order = displayAttribute.GetOrder();
                if (order.HasValue)
                {
                    return displayAttribute.GetOrder();
                }
            }

            List<PropertyOrderAttribute> list = pd.Attributes.OfType<PropertyOrderAttribute>().ToList();

            if (list.Count > 0)
            {
                this.ValidatePropertyOrderAttributes(list);

                if (this.IsPropertyGridCategorized)
                {
                    var attribute = list.FirstOrDefault(x => ((x.UsageContext == UsageContextEnum.Categorized) || (x.UsageContext == UsageContextEnum.Both)));
                    if (attribute != null)
                    {
                        return attribute.Order;
                    }
                }
                else
                {
                    var attribute = list.FirstOrDefault(x => ((x.UsageContext == UsageContextEnum.Alphabetical) || (x.UsageContext == UsageContextEnum.Both)));
                    if (attribute != null)
                    {
                        return attribute.Order;
                    }
                }
            }
            
            return int.MaxValue;
        }

        internal object ComputeExpandableAttributeForItem(object item)
        {
            var pd = (PropertyDescriptor)item;

            var attribute = PropertyGridUtilities.GetAttribute<ExpandableObjectAttribute>(pd);
            return (attribute != null);
        }

        internal int ComputeDisplayOrderInternal(bool isPropertyGridCategorized)
        {
            return this.ComputeDisplayOrder(isPropertyGridCategorized);
        }

        internal object GetValueInstance(object sourceObject)
        {
            ICustomTypeDescriptor customTypeDescriptor = sourceObject as ICustomTypeDescriptor;
            if (customTypeDescriptor != null)
            {
                sourceObject = customTypeDescriptor.GetPropertyOwner(PropertyDescriptor);
            }

            return sourceObject;
        }

        internal object ComputeDefaultValueAttributeForItem(object item)
        {
            var pd = (PropertyDescriptor)item;

            var defaultValue = PropertyGridUtilities.GetAttribute<DefaultValueAttribute>(pd);
            return (defaultValue != null) ? defaultValue.Value : null;
        }

        private static void ExecuteResetValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var affectedPropertyItem = e.Parameter as PropertyItem;
            if (affectedPropertyItem == null)
            {
                affectedPropertyItem = sender as PropertyItem;
            }

            if ((affectedPropertyItem != null) && (affectedPropertyItem.DescriptorDefinition != null))
            {
                if (affectedPropertyItem.DescriptorDefinition.ComputeCanResetValue())
                {
                    affectedPropertyItem.DescriptorDefinition.ResetValue();
                }
            }
        }

        private static void CanExecuteResetValueCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var affectedPropertyItem = e.Parameter as PropertyItem;
            if (affectedPropertyItem == null)
            {
                affectedPropertyItem = sender as PropertyItem;
            }

            e.CanExecute = ((affectedPropertyItem != null) && (affectedPropertyItem.DescriptorDefinition != null)) ? affectedPropertyItem.DescriptorDefinition.ComputeCanResetValue() : false;
        }

        private string ComputeDisplayName()
        {
            var displayAttribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(PropertyDescriptor);
            var displayName = (displayAttribute != null) ? displayAttribute.GetName() : PropertyDescriptor.DisplayName;

            var attribute = PropertyGridUtilities.GetAttribute<ParenthesizePropertyNameAttribute>(PropertyDescriptor);
            if ((attribute != null) && attribute.NeedParenthesis)
            {
                displayName = "(" + displayName + ")";
            }

            return displayName;
        }

        private void ValidatePropertyOrderAttributes(List<PropertyOrderAttribute> list)
        {
            if (list.Count > 0)
            {
                PropertyOrderAttribute both = list.FirstOrDefault(x => x.UsageContext == UsageContextEnum.Both);
                if ((both != null) && (list.Count > 1))
                {
                    Debug.Assert(false, "A PropertyItem can't have more than 1 PropertyOrderAttribute when it has UsageContext : Both");
                }
            }
        }

        public event EventHandler ContainerHelperInvalidated;

        public static readonly DependencyProperty AdvancedOptionsIconProperty =
            DependencyProperty.Register("AdvancedOptionsIcon", typeof(ImageSource), typeof(DescriptorPropertyDefinitionBase), new UIPropertyMetadata(null));
        public ImageSource AdvancedOptionsIcon
        {
            get
            {
                return (ImageSource)GetValue(AdvancedOptionsIconProperty);
            }
            set
            {
                SetValue(AdvancedOptionsIconProperty, value);
            }
        }

        public static readonly DependencyProperty AdvancedOptionsTooltipProperty =
            DependencyProperty.Register("AdvancedOptionsTooltip", typeof(object), typeof(DescriptorPropertyDefinitionBase), new UIPropertyMetadata(null));
        public object AdvancedOptionsTooltip
        {
            get
            {
                return (object)GetValue(AdvancedOptionsTooltipProperty);
            }
            set
            {
                SetValue(AdvancedOptionsTooltipProperty, value);
            }
        }

        public static readonly DependencyProperty IsExpandableProperty =
            DependencyProperty.Register("IsExpandable", typeof(bool), typeof(DescriptorPropertyDefinitionBase), new UIPropertyMetadata(false));
        public bool IsExpandable
        {
            get
            {
                return (bool)GetValue(IsExpandableProperty);
            }
            set
            {
                SetValue(IsExpandableProperty, value);
            }
        }

        internal bool IsPropertyGridCategorized
        {
            get;
            set;
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DescriptorPropertyDefinitionBase), new UIPropertyMetadata(null, OnValueChanged));
        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DescriptorPropertyDefinitionBase)o).OnValueChanged(e.OldValue, e.NewValue);
        }

        internal virtual void OnValueChanged(object oldValue, object newValue)
        {
            UpdateIsExpandable();
            UpdateAdvanceOptions();
            CommandManager.InvalidateRequerySuggested();
        }

        public virtual void InitProperties()
        {
            isReadOnly = ComputeIsReadOnly();
            category = ComputeCategory();
            categoryValue = ComputeCategoryValue();
            description = ComputeDescription();
            displayName = ComputeDisplayName();
            defaultValue = ComputeDefaultValueAttribute();
            displayOrder = ComputeDisplayOrder(this.IsPropertyGridCategorized);
            expandableAttribute = ComputeExpandableAttribute();

            newItemTypes = ComputeNewItemTypes();
            commandBindings = new CommandBinding[] { new CommandBinding(PropertyItemCommands.ResetValue, ExecuteResetValueCommand, CanExecuteResetValueCommand) };

            BindingBase valueBinding = this.CreateValueBinding();
            BindingOperations.SetBinding(this, DescriptorPropertyDefinitionBase.ValueProperty, valueBinding);
        }
    }
}
