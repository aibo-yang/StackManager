using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Common.UI.WPF.PropertyGrid.Converters
{
    //[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    //public class EnumDisplayNameAttribute : Attribute
    //{
    //    public EnumDisplayNameAttribute(string displayName)
    //    {
    //        DisplayName = displayName;
    //    }

    //    public string DisplayName { get; set; }
    //}

    public class EnumMapper
    {
        public EnumMapper(object enumValue, string enumDescription)
        {
            Enum = enumValue;
            Description = enumDescription;
        }

        public object Enum { get; private set; }
        public string Description { get; private set; }
    }

    public class EnumTypeConverter : EnumConverter
    {
        private readonly IEnumerable<EnumMapper> mappings;

        public EnumTypeConverter(Type enumType)
            : base(enumType)
        {
            mappings = from object enumValue in Enum.GetValues(enumType)
                       select new EnumMapper(enumValue, GetDisplayName(enumValue));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
            {
                var enumType = value.GetType();
                if (enumType.IsEnum)
                {
                    return GetDisplayName(value);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var match = mappings.FirstOrDefault(mapping => string.Compare(mapping.Description, (string)value, true, culture) == 0);
                if (match != null)
                {
                    return match.Enum;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        private string GetDisplayName(object enumValue)
        {
            var displayNameAttribute = EnumType.GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            if (displayNameAttribute != null)
            {
                return displayNameAttribute.Name;
            }

            return Enum.GetName(EnumType, enumValue);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            TypeConverter.StandardValuesCollection stdValues = base.GetStandardValues(context);

            List<string> retVal = new List<string>();
            foreach (object value in stdValues)
            {
                retVal.Add(GetDisplayName(value));
            }

            return new TypeConverter.StandardValuesCollection(retVal);
        }
    }
}
