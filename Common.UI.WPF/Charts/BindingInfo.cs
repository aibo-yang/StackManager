using System.Windows.Data;

namespace Common.UI.WPF.Charts
{
    public class BindingInfo
    {
        public Binding Binding { get; set; }
        public DataPointPropertyName PropertyName { get; set; } = DataPointPropertyName.None;

        public BindingInfo()
        {
        }

        public BindingInfo(Binding binding, DataPointPropertyName propertyName)
        {
            Binding = binding;
            PropertyName = propertyName;
        }
    }
}
