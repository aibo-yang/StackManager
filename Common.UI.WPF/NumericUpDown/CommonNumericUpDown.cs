using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace Common.UI.WPF
{
    public abstract class CommonNumericUpDown<T> : NumericUpDown<T?> where T : struct, IFormattable, IComparable<T>
    {
        protected delegate bool FromText(string s, NumberStyles style, IFormatProvider provider, out T result);
        protected delegate T FromDecimal(decimal d);

        private FromText _fromText;
        private FromDecimal _fromDecimal;

        private Func<T, T, bool> _fromLowerThan;
        private Func<T, T, bool> _fromGreaterThan;

        internal static readonly DependencyProperty IsInvalidProperty = DependencyProperty.Register("IsInvalid", typeof(bool), typeof(CommonNumericUpDown<T>), new UIPropertyMetadata(false));
        internal bool IsInvalid
        {
            get
            {
                return (bool)GetValue(IsInvalidProperty);
            }
            private set
            {
                SetValue(IsInvalidProperty, value);
            }
        }
        public static readonly DependencyProperty ParsingNumberStyleProperty =
            DependencyProperty.Register("ParsingNumberStyle", typeof(NumberStyles), typeof(CommonNumericUpDown<T>), new UIPropertyMetadata(NumberStyles.Any));
       
        public NumberStyles ParsingNumberStyle
        {
            get { return (NumberStyles)GetValue(ParsingNumberStyleProperty); }
            set { SetValue(ParsingNumberStyleProperty, value); }
        }
        protected CommonNumericUpDown(FromText fromText, FromDecimal fromDecimal, Func<T, T, bool> fromLowerThan, Func<T, T, bool> fromGreaterThan)
        {
            if (fromText == null)
                throw new ArgumentNullException("tryParseMethod");
            if (fromDecimal == null)
                throw new ArgumentNullException("fromDecimal");
            if (fromLowerThan == null)
                throw new ArgumentNullException("fromLowerThan");
            if (fromGreaterThan == null)
                throw new ArgumentNullException("fromGreaterThan");

            _fromText = fromText;
            _fromDecimal = fromDecimal;
            _fromLowerThan = fromLowerThan;
            _fromGreaterThan = fromGreaterThan;
        }
        protected static void UpdateMetadata(Type type, T? increment, T? minValue, T? maxValue)
        {
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            UpdateMetadataCommon(type, increment, minValue, maxValue);
        }
        protected void TestInputSpecialValue(AllowedSpecialValues allowedValues, AllowedSpecialValues valueToCompare)
        {
            if ((allowedValues & valueToCompare) != valueToCompare)
            {
                switch (valueToCompare)
                {
                    case AllowedSpecialValues.NaN:
                        throw new InvalidDataException("Value to parse shouldn't be NaN.");
                    case AllowedSpecialValues.PositiveInfinity:
                        throw new InvalidDataException("Value to parse shouldn't be Positive Infinity.");
                    case AllowedSpecialValues.NegativeInfinity:
                        throw new InvalidDataException("Value to parse shouldn't be Negative Infinity.");
                }
            }
        }

        internal bool IsBetweenMinMax(T? value)
        {
            return !IsLowerThan(value, Minimum) && !IsGreaterThan(value, Maximum);
        }

        private static void UpdateMetadataCommon(Type type, T? increment, T? minValue, T? maxValue)
        {
            IncrementProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(increment));
            MaximumProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(maxValue));
            MinimumProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(minValue));
        }

        private bool IsLowerThan(T? value1, T? value2)
        {
            if (value1 == null || value2 == null)
                return false;
            return _fromLowerThan(value1.Value, value2.Value);
        }

        private bool IsGreaterThan(T? value1, T? value2)
        {
            if (value1 == null || value2 == null)
                return false;
            return _fromGreaterThan(value1.Value, value2.Value);
        }

        private bool HandleNullSpin()
        {
            var hasValue = this.UpdateValueOnEnterKey
                           ? (this.ConvertTextToValue(this.TextBox.Text) != null)
                           : this.Value.HasValue;
            if (!hasValue)
            {
                var forcedValue = this.DefaultValue.HasValue ? this.DefaultValue.Value : default(T);
                var newValue = CoerceValueMinMax(forcedValue);
                if (this.UpdateValueOnEnterKey)
                {
                    this.TextBox.Text = newValue.Value.ToString(this.FormatString, this.CultureInfo);
                }
                else
                {
                    this.Value = newValue;
                }
                return true;
            }
            else if (!this.Increment.HasValue)
            {
                return true;
            }
            return false;
        }

        private T? CoerceValueMinMax(T value)
        {
            if (IsLowerThan(value, Minimum))
                return Minimum;
            else if (IsGreaterThan(value, Maximum))
                return Maximum;
            else
                return value;
        }

        protected override void OnIncrement()
        {
            if (!HandleNullSpin())
            {
                if (this.UpdateValueOnEnterKey)
                {
                    var currentValue = this.ConvertTextToValue(this.TextBox.Text);
                    var result = this.IncrementValue(currentValue.Value, Increment.Value);
                    var newValue = this.CoerceValueMinMax(result);
                    this.TextBox.Text = newValue.Value.ToString(this.FormatString, this.CultureInfo);
                }
                else
                {
                    var result = this.IncrementValue(Value.Value, Increment.Value);
                    this.Value = this.CoerceValueMinMax(result);
                }
            }
        }

        protected override void OnDecrement()
        {
            if (!HandleNullSpin())
            {
                if (this.UpdateValueOnEnterKey)
                {
                    var currentValue = this.ConvertTextToValue(this.TextBox.Text);
                    var result = this.DecrementValue(currentValue.Value, Increment.Value);
                    var newValue = this.CoerceValueMinMax(result);
                    this.TextBox.Text = newValue.Value.ToString(this.FormatString, this.CultureInfo);
                }
                else
                {
                    var result = this.DecrementValue(Value.Value, Increment.Value);
                    this.Value = this.CoerceValueMinMax(result);
                }
            }
        }

        protected override void OnMinimumChanged(T? oldValue, T? newValue)
        {
            base.OnMinimumChanged(oldValue, newValue);
            if (this.Value.HasValue && this.ClipValueToMinMax)
            {
                this.Value = this.CoerceValueMinMax(this.Value.Value);
            }
        }

        protected override void OnMaximumChanged(T? oldValue, T? newValue)
        {
            base.OnMaximumChanged(oldValue, newValue);
            if (this.Value.HasValue && this.ClipValueToMinMax)
            {
                this.Value = this.CoerceValueMinMax(this.Value.Value);
            }
        }

        protected override T? ConvertTextToValue(string text)
        {
            T? result = null;
            if (String.IsNullOrEmpty(text))
                return result;
            string currentValueText = ConvertValueToText();
            if (object.Equals(currentValueText, text))
            {
                this.IsInvalid = false;
                return this.Value;
            }
            result = this.ConvertTextToValueCore(currentValueText, text);
            if (this.ClipValueToMinMax)
            {
                return this.GetClippedMinMaxValue(result);
            }
            ValidateDefaultMinMax(result);
            return result;
        }

        protected override string ConvertValueToText()
        {
            if (Value == null)
                return string.Empty;
            this.IsInvalid = false;
            if (FormatString.Contains("{0"))
                return string.Format(CultureInfo, FormatString, Value.Value);
            return Value.Value.ToString(FormatString, CultureInfo);
        }

        protected override void SetValidSpinDirection()
        {
            ValidSpinDirections validDirections = ValidSpinDirections.None;
            if ((this.Increment != null) && !IsReadOnly)
            {
                if (IsLowerThan(Value, Maximum) || !Value.HasValue || !Maximum.HasValue)
                    validDirections = validDirections | ValidSpinDirections.Increase;
                if (IsGreaterThan(Value, Minimum) || !Value.HasValue || !Minimum.HasValue)
                    validDirections = validDirections | ValidSpinDirections.Decrease;
            }
            if (Spinner != null)
                Spinner.ValidSpinDirection = validDirections;
        }

        private bool IsPercent(string stringToTest)
        {
            int PIndex = stringToTest.IndexOf("P");
            if (PIndex >= 0)
            {
                bool isText = (stringToTest.Substring(0, PIndex).Contains("'")
                              && stringToTest.Substring(PIndex, FormatString.Length - PIndex).Contains("'"));
                return !isText;
            }
            return false;
        }

        private T? ConvertTextToValueCore(string currentValueText, string text)
        {
            T? result;
            if (this.IsPercent(this.FormatString))
            {
                result = _fromDecimal(ParsePercent(text, CultureInfo));
            }
            else
            {
                T outputValue = new T();
                if (!_fromText(text, this.ParsingNumberStyle, CultureInfo, out outputValue))
                {
                    bool shouldThrow = true;
                    T currentValueTextOutputValue;
                    if (!_fromText(currentValueText, this.ParsingNumberStyle, CultureInfo, out currentValueTextOutputValue))
                    {
                        var currentValueTextSpecialCharacters = currentValueText.Where(c => !Char.IsDigit(c));
                        if (currentValueTextSpecialCharacters.Count() > 0)
                        {
                            var textSpecialCharacters = text.Where(c => !Char.IsDigit(c));
                            if (currentValueTextSpecialCharacters.Except(textSpecialCharacters).ToList().Count == 0)
                            {
                                foreach (var character in textSpecialCharacters)
                                {
                                    text = text.Replace(character.ToString(), string.Empty);
                                }
                                if (_fromText(text, this.ParsingNumberStyle, CultureInfo, out outputValue))
                                {
                                    shouldThrow = false;
                                }
                            }
                        }
                    }
                    if (shouldThrow)
                    {
                        this.IsInvalid = true;
                        throw new InvalidDataException("Input string was not in a correct format.");
                    }
                }
                result = outputValue;
            }
            return result;
        }

        private T? GetClippedMinMaxValue(T? result)
        {
            if (this.IsGreaterThan(result, this.Maximum))
                return this.Maximum;
            else if (this.IsLowerThan(result, this.Minimum))
                return this.Minimum;
            return result;
        }

        private void ValidateDefaultMinMax(T? value)
        {
            if (object.Equals(value, DefaultValue))
                return;
            if (IsLowerThan(value, Minimum))
                throw new ArgumentOutOfRangeException("Minimum", String.Format("Value must be greater than MinValue of {0}", Minimum));
            else if (IsGreaterThan(value, Maximum))
                throw new ArgumentOutOfRangeException("Maximum", String.Format("Value must be less than MaxValue of {0}", Maximum));
        }

        protected abstract T IncrementValue(T value, T increment);
        protected abstract T DecrementValue(T value, T increment);
    }
}
