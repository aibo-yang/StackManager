using System.Windows;
using System.Windows.Data;
using Common.UI.WPF.Primitives;

namespace Common.UI.WPF.PropertyGrid.Editors
{
    public abstract class TypeEditor<T> : ITypeEditor where T : FrameworkElement, new()
    {
        #region Properties

        protected T Editor
        {
            get;
            set;
        }

        protected DependencyProperty ValueProperty
        {
            get;
            set;
        }

        #endregion //Properties

        #region ITypeEditor Members

        public virtual FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Editor = this.CreateEditor();
            SetValueDependencyProperty();
            SetControlProperties(propertyItem);
            ResolveValueBinding(propertyItem);
            return Editor;
        }

        #endregion //ITypeEditor Members

        #region Methods

        protected virtual T CreateEditor()
        {
            return new T();
        }

        protected virtual IValueConverter CreateValueConverter()
        {
            return null;
        }

        protected virtual void ResolveValueBinding(PropertyItem propertyItem)
        {
            var binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.UpdateSourceTrigger = (Editor is InputBase) ? UpdateSourceTrigger.PropertyChanged : UpdateSourceTrigger.Default;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            binding.Converter = CreateValueConverter();
            BindingOperations.SetBinding(Editor, ValueProperty, binding);
        }

        protected virtual void SetControlProperties(PropertyItem propertyItem)
        {
            //TODO: implement in derived class
            // Do not set Editor properties which could not be overriden in a user style.
        }

        protected abstract void SetValueDependencyProperty();

        #endregion //Methods
    }
}
