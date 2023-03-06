using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using Common.UI.WPF.Core.Utilities;

namespace Common.UI.WPF.PropertyGrid
{
    public abstract class LockedDependencyObject : DependencyObject
    {
        private bool isLocked;
        internal bool IsLocked
        { 
            get { return isLocked; }
        }

        internal void ThrowIfLocked<TMember>(Expression<Func<TMember>> propertyExpression)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            if (this.IsLocked)
            {
                var propertyName = ReflectionHelper.GetPropertyOrFieldName(propertyExpression);
                var message = $"Cannot modify {propertyName} once the definition has beed added to a collection.";
                throw new InvalidOperationException(message);
            }
        }

        internal virtual void Lock()
        {
            if (!isLocked)
            {
                isLocked = true;
            }
        }
    }
}
