using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Common.UI.WPF.PropertyGrid.Attributes;

namespace Common.UI.WPF.PropertyGrid
{
    internal class ObjectContainerHelper : ObjectContainerHelperBase
    {
        private object selectedObject;
        private object SelectedObject
        {
            get
            {
                return selectedObject;
            }
        }

        public ObjectContainerHelper(IPropertyContainer propertyContainer, object selectedObject)
          : base(propertyContainer)
        {
            this.selectedObject = selectedObject;
        }

        protected override string GetDefaultPropertyName()
        {
            object selectedObject = SelectedObject;
            return (selectedObject != null) ? ObjectContainerHelperBase.GetDefaultPropertyName(SelectedObject) : (string)null;
        }

        protected override void GenerateSubPropertiesCore(Action<IEnumerable<PropertyItem>> updatePropertyItemsCallback)
        {
            var propertyItems = new List<PropertyItem>();

            if (SelectedObject != null)
            {
                try
                {
                    var descriptors = new List<PropertyDescriptor>();
                    {
                        descriptors = ObjectContainerHelperBase.GetPropertyDescriptors(SelectedObject, this.PropertyContainer.HideInheritedProperties);
                    }

                    foreach (var descriptor in descriptors)
                    {
                        var propertyDef = this.GetPropertyDefinition(descriptor);
                        bool isBrowsable = false;

                        var isPropertyBrowsable = this.PropertyContainer.IsPropertyVisible(descriptor);
                        if (isPropertyBrowsable.HasValue)
                        {
                            isBrowsable = isPropertyBrowsable.Value;
                        }
                        else
                        {
                            var displayAttribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(descriptor);
                            if (displayAttribute != null)
                            {
                                var autoGenerateField = displayAttribute.GetAutoGenerateField();
                                isBrowsable = this.PropertyContainer.AutoGenerateProperties && ((autoGenerateField.HasValue && autoGenerateField.Value) || !autoGenerateField.HasValue);
                            }
                            else
                            {
                                isBrowsable = descriptor.IsBrowsable && this.PropertyContainer.AutoGenerateProperties;
                            }

                            if (propertyDef != null)
                            {
                                isBrowsable = propertyDef.IsBrowsable.GetValueOrDefault(isBrowsable);
                            }
                        }

                        if (isBrowsable)
                        {
                            var prop = this.CreatePropertyItem(descriptor, propertyDef);
                            if (prop != null)
                            {
                                propertyItems.Add(prop);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Property creation failed.");
                    Debug.WriteLine(e.StackTrace);
                }
            }

            updatePropertyItemsCallback.Invoke(propertyItems);
        }


        private PropertyItem CreatePropertyItem(PropertyDescriptor property, PropertyDefinition propertyDef)
        {
            DescriptorPropertyDefinition definition = new DescriptorPropertyDefinition(property, SelectedObject, this.PropertyContainer);
            definition.InitProperties();

            this.InitializeDescriptorDefinition(definition, propertyDef);
            PropertyItem propertyItem = new PropertyItem(definition);
            Debug.Assert(SelectedObject != null);
            propertyItem.Instance = SelectedObject;
            propertyItem.CategoryOrder = this.GetCategoryOrder(definition.CategoryValue);

            propertyItem.WillRefreshPropertyGrid = this.GetWillRefreshPropertyGrid(property);
            return propertyItem;
        }

        private int GetCategoryOrder(object categoryValue)
        {
            Debug.Assert(this.SelectedObject != null);

            if (categoryValue == null)
            {
                return int.MaxValue;
            }

            int order = int.MaxValue;
            var orderAttribute = TypeDescriptor.GetAttributes(this.SelectedObject).OfType<CategoryOrderAttribute>().FirstOrDefault(attribute => Equals(attribute.CategoryValue, categoryValue));

            if (orderAttribute != null)
            {
                order = orderAttribute.Order;
            }

            return order;
        }
    }
}
