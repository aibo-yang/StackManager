using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Common.UI.WPF.PropertyGrid.Attributes;
using Common.UI.WPF.PropertyGrid.Editors;

namespace Common.UI.WPF.PropertyGrid
{
    internal class DescriptorPropertyDefinition : DescriptorPropertyDefinitionBase
    {
        private object selectedObject;
        private object SelectedObject
        {
            get
            {
                return selectedObject;
            }
        }

        private PropertyDescriptor propertyDescriptor;
        internal override PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return propertyDescriptor;
            }
        }

        private DependencyPropertyDescriptor dpDescriptor;
        private static Dictionary<string, Type> dictEditorTypeName = new Dictionary<string, Type>();

        internal DescriptorPropertyDefinition(PropertyDescriptor propertyDescriptor, object selectedObject, IPropertyContainer propertyContainer)
           : base(propertyContainer.IsCategorized)
        {
            this.Init(propertyDescriptor, selectedObject);
        }

        private void Init(PropertyDescriptor propertyDescriptor, object selectedObject)
        {
            this.propertyDescriptor = propertyDescriptor ?? throw new ArgumentNullException(nameof(propertyDescriptor));
            this.selectedObject = selectedObject ?? throw new ArgumentNullException(nameof(selectedObject));
            this.dpDescriptor = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);
        }

        internal override ObjectContainerHelperBase CreateContainerHelper(IPropertyContainer parent)
        {
            return new ObjectContainerHelper(parent, this.Value);
        }

        internal override void OnValueChanged(object oldValue, object newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            this.RaiseContainerHelperInvalidated();
        }

        protected override BindingBase CreateValueBinding()
        {
            var selectedObject = this.SelectedObject;
            var propertyName = this.PropertyDescriptor.Name;
            
            var binding = new Binding(propertyName)
            {
                Source = this.GetValueInstance(selectedObject),
                Mode = PropertyDescriptor.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                ValidatesOnDataErrors = true,
                ValidatesOnExceptions = true,
                ConverterCulture = CultureInfo.CurrentCulture
            };

            return binding;
        }

        protected override bool ComputeIsReadOnly()
        {
            return PropertyDescriptor.IsReadOnly;
        }

        internal override ITypeEditor CreateDefaultEditor(PropertyItem propertyItem)
        {
            return PropertyGridUtilities.CreateDefaultEditor(PropertyDescriptor.PropertyType, PropertyDescriptor.Converter, propertyItem);
        }

        protected override bool ComputeCanResetValue()
        {
            if (!PropertyDescriptor.IsReadOnly)
            {
                var defaultValue = this.ComputeDefaultValueAttribute();
                if (defaultValue != null)
                {
                    return !defaultValue.Equals(this.Value);
                }

                return PropertyDescriptor.CanResetValue(SelectedObject);
            }

            return false;
        }

        protected override object ComputeAdvancedOptionsTooltip()
        {
            UpdateAdvanceOptionsForItem(SelectedObject as DependencyObject, dpDescriptor, out object tooltip);
            return tooltip;
        }

        protected override string ComputeCategory()
        {
            var displayAttribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(PropertyDescriptor);
            if ((displayAttribute != null) && (displayAttribute.GetGroupName() != null))
            {
                return displayAttribute.GetGroupName();
            }
            else
            {
                var category = PropertyDescriptor.Category;
                //if (string.Equals(category, "Misc", StringComparison.OrdinalIgnoreCase))
                //{
                //    category = "杂项";
                //}
                return category;
            }
        }

        protected override string ComputeCategoryValue()
        {
            return PropertyDescriptor.Category;
        }

        protected override bool ComputeExpandableAttribute()
        {
            return (bool)this.ComputeExpandableAttributeForItem(PropertyDescriptor);
        }

        protected override object ComputeDefaultValueAttribute()
        {
            return this.ComputeDefaultValueAttributeForItem(PropertyDescriptor);
        }

        protected override bool ComputeIsExpandable()
        {
            return (this.Value != null);
        }

        protected override IList<Type> ComputeNewItemTypes()
        {
            return (IList<Type>)ComputeNewItemTypesForItem(PropertyDescriptor);
        }

        protected override string ComputeDescription()
        {
            return (string)ComputeDescriptionForItem(PropertyDescriptor);
        }

        protected override int ComputeDisplayOrder(bool isPropertyGridCategorized)
        {
            this.IsPropertyGridCategorized = isPropertyGridCategorized;
            return (int)ComputeDisplayOrderForItem(PropertyDescriptor);
        }

        protected override void ResetValue()
        {
            this.PropertyDescriptor.ResetValue(this.SelectedObject);
            base.ResetValue();
        }

        internal override ITypeEditor CreateAttributeEditor()
        {
            var editorAttribute = GetAttribute<EditorAttribute>();
            if (editorAttribute != null)
            {
                if (!dictEditorTypeName.TryGetValue(editorAttribute.EditorTypeName, out Type type))
                {
                    try
                    {
                        var typeDef = editorAttribute.EditorTypeName.Split(new char[] { ',' });
                        if (typeDef.Length >= 2)
                        {
                            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains(typeDef[1].Trim()));
                            if (assembly != null)
                            {
                                type = assembly.GetTypes().FirstOrDefault(t => (t != null) && (t.FullName != null) && t.FullName.Contains(typeDef[0]));
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

                    if (type == null)
                    {
                        type = Type.GetType(editorAttribute.EditorTypeName);
                    }
                    dictEditorTypeName.Add(editorAttribute.EditorTypeName, type);
                }

                if (typeof(ITypeEditor).IsAssignableFrom(type) && (type.GetConstructor(Array.Empty<Type>()) != null))
                {
                    var instance = Activator.CreateInstance(type) as ITypeEditor;
                    Debug.Assert(instance != null, "Type was expected to be ITypeEditor with public constructor.");
                    if (instance != null)
                    {
                        return instance;
                    }
                }
            }

            var itemsSourceAttribute = GetAttribute<ItemsSourceAttribute>();
            if (itemsSourceAttribute != null)
            {
                return new ItemsSourceAttributeEditor(itemsSourceAttribute);
            }

            return null;
        }
        
        private T GetAttribute<T>() where T : Attribute
        {
            return PropertyGridUtilities.GetAttribute<T>(PropertyDescriptor);
        }
    }
}
