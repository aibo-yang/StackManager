using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Common.UI.WPF
{
    public abstract class Spinner : Control
    {
        public static readonly DependencyProperty ValidSpinDirectionProperty = DependencyProperty.Register("ValidSpinDirection", typeof(ValidSpinDirections), typeof(Spinner), new PropertyMetadata(ValidSpinDirections.Increase | ValidSpinDirections.Decrease, OnValidSpinDirectionPropertyChanged));
        public ValidSpinDirections ValidSpinDirection
        {
            get
            {
                return (ValidSpinDirections)GetValue(ValidSpinDirectionProperty);
            }
            set
            {
                SetValue(ValidSpinDirectionProperty, value);
            }
        }

        private static void OnValidSpinDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner source = (Spinner)d;
            ValidSpinDirections oldvalue = (ValidSpinDirections)e.OldValue;
            ValidSpinDirections newvalue = (ValidSpinDirections)e.NewValue;
            source.OnValidSpinDirectionChanged(oldvalue, newvalue);
        }

        public event EventHandler<SpinEventArgs> Spin;

        public static readonly RoutedEvent SpinnerSpinEvent = EventManager.RegisterRoutedEvent("SpinnerSpin", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Spinner));

        public event RoutedEventHandler SpinnerSpin
        {
            add
            {
                AddHandler(SpinnerSpinEvent, value);
            }
            remove
            {
                RemoveHandler(SpinnerSpinEvent, value);
            }
        }

        protected Spinner()
        {
        }

        protected virtual void OnSpin(SpinEventArgs e)
        {
            ValidSpinDirections valid = e.Direction == SpinDirection.Increase ? ValidSpinDirections.Increase : ValidSpinDirections.Decrease;

            if ((ValidSpinDirection & valid) == valid)
            {
                EventHandler<SpinEventArgs> handler = Spin;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        protected virtual void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
        {
        }
    }
}
