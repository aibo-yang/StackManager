using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Common.UI.WPF.Core.Input;

namespace Common.UI.WPF.Primitives
{
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Spinner, Type = typeof(Spinner))]
    public abstract class UpDownBase<T> : InputBase, IValidateInput
    {
        internal const string PART_TextBox = "PART_TextBox";
        internal const string PART_Spinner = "PART_Spinner";

        internal bool isTextChangedFromUI;
        private bool isSyncingTextAndValueProperties;
        private bool internalValueSet;

        protected Spinner Spinner
        {
            get;
            private set;
        }

        protected TextBox TextBox
        {
            get;
            private set;
        }

        public static readonly DependencyProperty AllowSpinProperty = DependencyProperty.Register("AllowSpin", typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(true));
        public bool AllowSpin
        {
            get
            {
                return (bool)GetValue(AllowSpinProperty);
            }
            set
            {
                SetValue(AllowSpinProperty, value);
            }
        }

        public static readonly DependencyProperty ButtonSpinnerLocationProperty = DependencyProperty.Register("ButtonSpinnerLocation", typeof(Location), typeof(UpDownBase<T>), new UIPropertyMetadata(Location.Right));
        public Location ButtonSpinnerLocation
        {
            get
            {
                return (Location)GetValue(ButtonSpinnerLocationProperty);
            }
            set
            {
                SetValue(ButtonSpinnerLocationProperty, value);
            }
        }

        public static readonly DependencyProperty ClipValueToMinMaxProperty = DependencyProperty.Register("ClipValueToMinMax", typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(false));
        public bool ClipValueToMinMax
        {
            get
            {
                return (bool)GetValue(ClipValueToMinMaxProperty);
            }
            set
            {
                SetValue(ClipValueToMinMaxProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayDefaultValueOnEmptyTextProperty = DependencyProperty.Register("DisplayDefaultValueOnEmptyText", typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(false, OnDisplayDefaultValueOnEmptyTextChanged));
        public bool DisplayDefaultValueOnEmptyText
        {
            get
            {
                return (bool)GetValue(DisplayDefaultValueOnEmptyTextProperty);
            }
            set
            {
                SetValue(DisplayDefaultValueOnEmptyTextProperty, value);
            }
        }

        private static void OnDisplayDefaultValueOnEmptyTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((UpDownBase<T>)source).OnDisplayDefaultValueOnEmptyTextChanged((bool)args.OldValue, (bool)args.NewValue);
        }

        private void OnDisplayDefaultValueOnEmptyTextChanged(bool oldValue, bool newValue)
        {
            if (this.IsInitialized && string.IsNullOrEmpty(Text))
            {
                this.SyncTextAndValueProperties(false, Text);
            }
        }

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(T), typeof(UpDownBase<T>), new UIPropertyMetadata(default(T), OnDefaultValueChanged));
        public T DefaultValue
        {
            get
            {
                return (T)GetValue(DefaultValueProperty);
            }
            set
            {
                SetValue(DefaultValueProperty, value);
            }
        }

        private static void OnDefaultValueChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((UpDownBase<T>)source).OnDefaultValueChanged((T)args.OldValue, (T)args.NewValue);
        }

        private void OnDefaultValueChanged(T oldValue, T newValue)
        {
            if (this.IsInitialized && string.IsNullOrEmpty(Text))
            {
                this.SyncTextAndValueProperties(true, Text);
            }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(T), typeof(UpDownBase<T>), new UIPropertyMetadata(default(T), OnMaximumChanged, OnCoerceMaximum));
        public T Maximum
        {
            get
            {
                return (T)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> upDown = o as UpDownBase<T>;
            if (upDown != null)
                upDown.OnMaximumChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnMaximumChanged(T oldValue, T newValue)
        {
            if (this.IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        private static object OnCoerceMaximum(DependencyObject d, object baseValue)
        {
            UpDownBase<T> upDown = d as UpDownBase<T>;
            if (upDown != null)
                return upDown.OnCoerceMaximum((T)baseValue);
            return baseValue;
        }

        protected virtual T OnCoerceMaximum(T baseValue)
        {
            return baseValue;
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(T), typeof(UpDownBase<T>), new UIPropertyMetadata(default(T), OnMinimumChanged, OnCoerceMinimum));
        public T Minimum
        {
            get
            {
                return (T)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }

        private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> upDown = o as UpDownBase<T>;
            if (upDown != null)
                upDown.OnMinimumChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnMinimumChanged(T oldValue, T newValue)
        {
            if (this.IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        private static object OnCoerceMinimum(DependencyObject d, object baseValue)
        {
            UpDownBase<T> upDown = d as UpDownBase<T>;
            if (upDown != null)
                return upDown.OnCoerceMinimum((T)baseValue);
            return baseValue;
        }

        protected virtual T OnCoerceMinimum(T baseValue)
        {
            return baseValue;
        }

        public static readonly DependencyProperty MouseWheelActiveTriggerProperty = DependencyProperty.Register("MouseWheelActiveTrigger", typeof(MouseWheelActiveTrigger), typeof(UpDownBase<T>), new UIPropertyMetadata(MouseWheelActiveTrigger.FocusedMouseOver));
        public MouseWheelActiveTrigger MouseWheelActiveTrigger
        {
            get
            {
                return (MouseWheelActiveTrigger)GetValue(MouseWheelActiveTriggerProperty);
            }
            set
            {
                SetValue(MouseWheelActiveTriggerProperty, value);
            }
        }

        private static void OnMouseWheelActiveOnFocusChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> upDownBase = o as UpDownBase<T>;
            if (upDownBase != null)
                upDownBase.MouseWheelActiveTrigger = ((bool)e.NewValue)
                  ? MouseWheelActiveTrigger.FocusedMouseOver
                  : MouseWheelActiveTrigger.MouseOver;
        }

        public static readonly DependencyProperty ShowButtonSpinnerProperty = DependencyProperty.Register("ShowButtonSpinner", typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(true));
        public bool ShowButtonSpinner
        {
            get
            {
                return (bool)GetValue(ShowButtonSpinnerProperty);
            }
            set
            {
                SetValue(ShowButtonSpinnerProperty, value);
            }
        }

        public static readonly DependencyProperty UpdateValueOnEnterKeyProperty = DependencyProperty.Register("UpdateValueOnEnterKey", typeof(bool), typeof(UpDownBase<T>), new FrameworkPropertyMetadata(false, OnUpdateValueOnEnterKeyChanged));
        public bool UpdateValueOnEnterKey
        {
            get
            {
                return (bool)GetValue(UpdateValueOnEnterKeyProperty);
            }
            set
            {
                SetValue(UpdateValueOnEnterKeyProperty, value);
            }
        }

        private static void OnUpdateValueOnEnterKeyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var upDownBase = o as UpDownBase<T>;
            if (upDownBase != null)
                upDownBase.OnUpdateValueOnEnterKeyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnUpdateValueOnEnterKeyChanged(bool oldValue, bool newValue)
        {
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(T), typeof(UpDownBase<T>), new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, OnCoerceValue, false, UpdateSourceTrigger.PropertyChanged));
        public T Value
        {
            get
            {
                return (T)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        private void SetValueInternal(T value)
        {
            internalValueSet = true;
            try
            {
                this.Value = value;
            }
            finally
            {
                internalValueSet = false;
            }
        }

        private static object OnCoerceValue(DependencyObject o, object basevalue)
        {
            return ((UpDownBase<T>)o).OnCoerceValue(basevalue);
        }

        protected virtual object OnCoerceValue(object newValue)
        {
            return newValue;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> upDownBase = o as UpDownBase<T>;
            if (upDownBase != null)
                upDownBase.OnValueChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnValueChanged(T oldValue, T newValue)
        {
            if (!internalValueSet && this.IsInitialized)
            {
                SyncTextAndValueProperties(false, null, true);
            }
            SetValidSpinDirection();
            this.RaiseValueChangedEvent(oldValue, newValue);
        }

        internal UpDownBase()
        {
            this.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new RoutedEventHandler(this.HandleClickOutsideOfControlWithMouseCapture), true);
            this.IsKeyboardFocusWithinChanged += this.UpDownBase_IsKeyboardFocusWithinChanged;
        }

        protected override void OnAccessKey(AccessKeyEventArgs e)
        {
            if (TextBox != null)
            {
                TextBox.Focus();
            }
                
            base.OnAccessKey(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (TextBox != null)
            {
                TextBox.TextChanged -= new TextChangedEventHandler(TextBox_TextChanged);
                TextBox.RemoveHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(this.TextBox_PreviewMouseDown));
            }
            TextBox = GetTemplateChild(PART_TextBox) as TextBox;
            if (TextBox != null)
            {
                TextBox.Text = Text;
                TextBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
                TextBox.AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(this.TextBox_PreviewMouseDown), true);
            }
            if (Spinner != null)
            {
                Spinner.Spin -= OnSpinnerSpin;
            }
               
            Spinner = GetTemplateChild(PART_Spinner) as Spinner;
            if (Spinner != null)
            {
                Spinner.Spin += OnSpinnerSpin;
            }
                
            SetValidSpinDirection();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        bool commitSuccess = CommitInput();
                        e.Handled = !commitSuccess;
                        break;
                    }
            }
        }

        protected override void OnTextChanged(string oldValue, string newValue)
        {
            if (this.IsInitialized)
            {
                if (this.UpdateValueOnEnterKey)
                {
                    if (!isTextChangedFromUI)
                    {
                        this.SyncTextAndValueProperties(true, Text);
                    }
                }
                else
                {
                    this.SyncTextAndValueProperties(true, Text);
                }
            }
        }

        protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            if (IsInitialized)
            {
                SyncTextAndValueProperties(false, null);
            }
        }

        protected override void OnReadOnlyChanged(bool oldValue, bool newValue)
        {
            SetValidSpinDirection();
        }

        private void TextBox_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            if (this.MouseWheelActiveTrigger == Primitives.MouseWheelActiveTrigger.Focused)
            {
                if (Mouse.Captured != this.Spinner)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        Mouse.Capture(this.Spinner);
                    }
                    ));
                }
            }
        }

        private void HandleClickOutsideOfControlWithMouseCapture(object sender, RoutedEventArgs e)
        {
            if (Mouse.Captured is Spinner)
            {
                this.Spinner.ReleaseMouseCapture();
            }
        }

        private void OnSpinnerSpin(object sender, SpinEventArgs e)
        {
            if (AllowSpin && !IsReadOnly)
            {
                var activeTrigger = this.MouseWheelActiveTrigger;
                bool spin = !e.UsingMouseWheel;
                spin |= (activeTrigger == MouseWheelActiveTrigger.MouseOver);
                spin |= ((TextBox != null) && TextBox.IsFocused && (activeTrigger == MouseWheelActiveTrigger.FocusedMouseOver));
                spin |= ((TextBox != null) && TextBox.IsFocused && (activeTrigger == MouseWheelActiveTrigger.Focused) && (Mouse.Captured is Spinner));
                if (spin)
                {
                    e.Handled = true;
                    OnSpin(e);
                }
            }
        }

        public event InputValidationErrorEventHandler InputValidationError;
        public event EventHandler<SpinEventArgs> Spinned;

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(UpDownBase<T>));
        public event RoutedPropertyChangedEventHandler<object> ValueChanged
        {
            add
            {
                AddHandler(ValueChangedEvent, value);
            }
            remove
            {
                RemoveHandler(ValueChangedEvent, value);
            }
        }

        protected virtual void OnSpin(SpinEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");
            EventHandler<SpinEventArgs> handler = this.Spinned;
            if (handler != null)
            {
                handler(this, e);
            }
            if (e.Direction == SpinDirection.Increase)
                DoIncrement();
            else
                DoDecrement();
        }

        protected virtual void RaiseValueChangedEvent(T oldValue, T newValue)
        {
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue);
            args.RoutedEvent = ValueChangedEvent;
            RaiseEvent(args);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            bool updateValueFromText = (this.ReadLocalValue(ValueProperty) == DependencyProperty.UnsetValue) && (BindingOperations.GetBinding(this, ValueProperty) == null) && (object.Equals(this.Value, ValueProperty.DefaultMetadata.DefaultValue));
            this.SyncTextAndValueProperties(updateValueFromText, Text, !updateValueFromText);
        }

        internal void DoDecrement()
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
            {
                OnDecrement();
            }
        }

        internal void DoIncrement()
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
            {
                OnIncrement();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin)
                return;
            try
            {
                isTextChangedFromUI = true;
                Text = ((TextBox)sender).Text;
            }
            finally
            {
                isTextChangedFromUI = false;
            }
        }

        private void UpDownBase_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                this.CommitInput();
            }
        }

        private void RaiseInputValidationError(Exception e)
        {
            if (InputValidationError != null)
            {
                InputValidationErrorEventArgs args = new InputValidationErrorEventArgs(e);
                InputValidationError(this, args);
                if (args.ThrowException)
                {
                    throw args.Exception;
                }
            }
        }

        public virtual bool CommitInput()
        {
            return this.SyncTextAndValueProperties(true, Text);
        }

        protected bool SyncTextAndValueProperties(bool updateValueFromText, string text)
        {
            return this.SyncTextAndValueProperties(updateValueFromText, text, false);
        }

        private bool SyncTextAndValueProperties(bool updateValueFromText, string text, bool forceTextUpdate)
        {
            if (isSyncingTextAndValueProperties)
                return true;
            isSyncingTextAndValueProperties = true;
            bool parsedTextIsValid = true;
            try
            {
                if (updateValueFromText)
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        this.SetValueInternal(this.DefaultValue);
                    }
                    else
                    {
                        try
                        {
                            T newValue = this.ConvertTextToValue(text);
                            if (!object.Equals(newValue, this.Value))
                            {
                                this.SetValueInternal(newValue);
                            }
                        }
                        catch (Exception e)
                        {
                            parsedTextIsValid = false;
                            if (!isTextChangedFromUI)
                            {
                                this.RaiseInputValidationError(e);
                            }
                        }
                    }
                }
                if (!isTextChangedFromUI)
                {
                    bool shouldKeepEmpty = !forceTextUpdate && string.IsNullOrEmpty(Text) && object.Equals(Value, DefaultValue) && !this.DisplayDefaultValueOnEmptyText;
                    if (!shouldKeepEmpty)
                    {
                        string newText = ConvertValueToText();
                        if (!object.Equals(this.Text, newText))
                        {
                            Text = newText;
                        }
                    }
                    if (TextBox != null)
                        TextBox.Text = Text;
                }
                if (isTextChangedFromUI && !parsedTextIsValid)
                {
                    if (Spinner != null)
                    {
                        Spinner.ValidSpinDirection = ValidSpinDirections.None;
                    }
                }
                else
                {
                    this.SetValidSpinDirection();
                }
            }
            finally
            {
                isSyncingTextAndValueProperties = false;
            }
            return parsedTextIsValid;
        }

        protected abstract T ConvertTextToValue(string text);
        protected abstract string ConvertValueToText();
        protected abstract void OnIncrement();
        protected abstract void OnDecrement();
        protected abstract void SetValidSpinDirection();
    }
}
