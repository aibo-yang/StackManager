﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.UI.WPF.Core.Utilities
{
    internal class ListUtilities
    {
        internal static bool IsListOfItems(Type listType)
        {
            return (ListUtilities.GetListItemType(listType) != null);
        }

        internal static Type GetListItemType(Type listType)
        {
            Type iListOfT = listType.GetInterfaces().FirstOrDefault(
              (i) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));

            return (iListOfT != null)
              ? iListOfT.GetGenericArguments()[0]
              : null;
        }

        internal static bool IsCollectionOfItems(Type colType)
        {
            return (ListUtilities.GetCollectionItemType(colType) != null);
        }

        internal static Type GetCollectionItemType(Type colType)
        {
            Type iCollectionOfT = null;
            var isCollectionOfT = colType.IsGenericType && (colType.GetGenericTypeDefinition() == typeof(ICollection<>));
            if (isCollectionOfT)
            {
                iCollectionOfT = colType;
            }
            else
            {
                iCollectionOfT = colType.GetInterfaces().FirstOrDefault((i) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
            }

            return (iCollectionOfT != null)
              ? iCollectionOfT.GetGenericArguments()[0]
              : null;
        }

        internal static bool IsDictionaryOfItems(Type dictType)
        {
            return (ListUtilities.GetDictionaryItemsType(dictType) != null);
        }

        internal static Type[] GetDictionaryItemsType(Type dictType)
        {
            var isDict = dictType.IsGenericType
              && ((dictType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) || (dictType.GetGenericTypeDefinition() == typeof(IDictionary<,>)));

            return isDict
              ? new Type[] { dictType.GetGenericArguments()[0], dictType.GetGenericArguments()[1] }
              : null;
        }

        internal static object CreateEditableKeyValuePair(object key, Type keyType, object value, Type valueType)
        {
            var itemType = ListUtilities.CreateEditableKeyValuePairType(keyType, valueType);
            return Activator.CreateInstance(itemType, key, value);
        }

        internal static Type CreateEditableKeyValuePairType(Type keyType, Type valueType)
        {
            //return an EditableKeyValuePair< TKey, TValue> Type from keyType and valueType
            var itemGenType = typeof(EditableKeyValuePair<,>);
            Type[] itemGenTypeArgs = { keyType, valueType };
            return itemGenType.MakeGenericType(itemGenTypeArgs);
        }
    }
}
