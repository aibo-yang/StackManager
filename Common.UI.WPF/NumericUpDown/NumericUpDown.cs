using System;
using System.Globalization;
using System.Windows;
using Common.UI.WPF.Primitives;
namespace Common.UI.WPF
{
    public abstract class NumericUpDown<T> : UpDownBase<T>
    {
        public static readonly DependencyProperty AutoMoveFocusProperty = DependencyProperty.Register("AutoMoveFocus", typeof(bool), typeof(NumericUpDown<T>), new UIPropertyMetadata(false));
        public bool AutoMoveFocus
        {
            get
            {
                return (bool)GetValue(AutoMoveFocusProperty);
            }
            set
            {
                SetValue(AutoMoveFocusProperty, value);
            }
        }

        public static readonly DependencyProperty AutoSelectBehaviorProperty =
            DependencyProperty.Register("AutoSelectBehavior", typeof(AutoSelectBehavior), typeof(NumericUpDown<T>), new UIPropertyMetadata(AutoSelectBehavior.OnFocus));

        public AutoSelectBehavior AutoSelectBehavior
        {
            get
            {
                return (AutoSelectBehavior)GetValue(AutoSelectBehaviorProperty);
            }
            set
            {
                SetValue(AutoSelectBehaviorProperty, value);
            }
        }

        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString", typeof(string), typeof(NumericUpDown<T>), new UIPropertyMetadata(String.Empty, OnFormatStringChanged, OnCoerceFormatString));
       
        public string FormatString
        {
            get
            {
                return (string)GetValue(FormatStringProperty);
            }
            set
            {
                SetValue(FormatStringProperty, value);
            }
        }

        private static object OnCoerceFormatString(DependencyObject o, object baseValue)
        {
            if (o is NumericUpDown<T> numericUpDown)
            {
                return numericUpDown.OnCoerceFormatString((string)baseValue);
            }
            return baseValue;
        }
        protected virtual string OnCoerceFormatString(string baseValue)
        {
            return baseValue ?? string.Empty;
        }

        private static void OnFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is NumericUpDown<T> numericUpDown)
            { 
                numericUpDown.OnFormatStringChanged((string)e.OldValue, (string)e.NewValue);
            }
        }

        protected virtual void OnFormatStringChanged(string oldValue, string newValue)
        {
            if (IsInitialized)
            {
                this.SyncTextAndValueProperties(false, null);
            }
        }

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(T), typeof(NumericUpDown<T>), new PropertyMetadata(default(T), OnIncrementChanged, OnCoerceIncrement));
        public T Increment
        {
            get
            {
                return (T)GetValue(IncrementProperty);
            }
            set
            {
                SetValue(IncrementProperty, value);
            }
        }

        private static void OnIncrementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is NumericUpDown<T> numericUpDown)
            {
                numericUpDown.OnIncrementChanged((T)e.OldValue, (T)e.NewValue);
            }
        }

        protected virtual void OnIncrementChanged(T oldValue, T newValue)
        {
            if (this.IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        private static object OnCoerceIncrement(DependencyObject d, object baseValue)
        {
            if (d is NumericUpDown<T> numericUpDown)
            {
                return numericUpDown.OnCoerceIncrement((T)baseValue);
            }
            return baseValue;
        }

        protected virtual T OnCoerceIncrement(T baseValue)
        {
            return baseValue;
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(NumericUpDown<T>), new UIPropertyMetadata(0));
        public int MaxLength
        {
            get
            {
                return (int)GetValue(MaxLengthProperty);
            }
            set
            {
                SetValue(MaxLengthProperty, value);
            }
        }

        protected static decimal ParsePercent(string text, IFormatProvider cultureInfo)
        {
            NumberFormatInfo info = NumberFormatInfo.GetInstance(cultureInfo);
            text = text.Replace(info.PercentSymbol, null);
            decimal result = Decimal.Parse(text, NumberStyles.Any, info);
            result = result / 100;
            return result;
        }
    }
}
