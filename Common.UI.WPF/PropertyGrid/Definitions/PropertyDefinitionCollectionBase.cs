using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.UI.WPF.PropertyGrid
{
    public abstract class PropertyDefinitionCollectionBase<T> : LockedObservableCollection<T> where T : PropertyDefinitionBase
    {
        protected PropertyDefinitionCollectionBase()
        {
        }

        public virtual T this[object propertyId]
        {
            get
            {
                foreach (var item in Items)
                {
                    if (item.TargetProperties.Contains(propertyId))
                    {
                        return item;
                    }

                    List<string> stringTargetProperties = item.TargetProperties.OfType<string>().ToList();
                    if ((stringTargetProperties != null) && (stringTargetProperties.Count > 0))
                    {
                        if (propertyId is string stringPropertyID)
                        {
                            foreach (var targetPropertyString in stringTargetProperties)
                            {
                                if (targetPropertyString.Contains("*"))
                                {
                                    string searchString = targetPropertyString.Replace("*", "");
                                    if (stringPropertyID.StartsWith(searchString) || stringPropertyID.EndsWith(searchString))
                                    {
                                        return item;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Manage Interfaces
                        var type = propertyId as Type;
                        if (type != null)
                        {
                            foreach (Type targetProperty in item.TargetProperties)
                            {
                                if (targetProperty.IsAssignableFrom(type))
                                {
                                    return item;
                                }
                            }
                        }
                    }
                }

                return null;
            }
        }

        internal T GetRecursiveBaseTypes(Type type)
        {
            // If no definition for the current type, fall back on base type editor recursively.
            T ret = null;
            while (ret == null && type != null)
            {
                ret = this[type];
                type = type.BaseType;
            }
            return ret;
        }
    }
}
