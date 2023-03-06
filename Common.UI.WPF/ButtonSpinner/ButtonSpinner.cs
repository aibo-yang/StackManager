using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace Common.UI.WPF
{
    public enum Location
    {
        Left,
        Right
    }

    [TemplatePart(Name = PART_IncreaseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_DecreaseButton, Type = typeof(ButtonBase))]
    [ContentProperty("Content")]
    public class ButtonSpinner : Spinner
    {
        private const string PART_IncreaseButton = "PART_IncreaseButton";
        private const string PART_DecreaseButton = "PART_DecreaseButton";
        
        public static readonly DependencyProperty AllowSpinProperty = DependencyProperty.Register("AllowSpin", typeof(bool), typeof(ButtonSpinner), new UIPropertyMetadata(true, AllowSpinPropertyChanged));
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

        private static void AllowSpinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonSpinner source = d as ButtonSpinner;
            source.OnAllowSpinChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        public static readonly DependencyProperty ButtonSpinnerLocationProperty = DependencyProperty.Register("ButtonSpinnerLocation", typeof(Location), typeof(ButtonSpinner), new UIPropertyMetadata(Location.Right));
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

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(ButtonSpinner), new PropertyMetadata(null, OnContentPropertyChanged));
        public object Content
        {
            get
            {
                return GetValue(ContentProperty) as object;
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }
        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonSpinner source = d as ButtonSpinner;
            source.OnContentChanged(e.OldValue, e.NewValue);
        }

        private ButtonBase decreaseButton;
        private ButtonBase DecreaseButton
        {
            get
            {
                return decreaseButton;
            }
            set
            {
                if (decreaseButton != null)
                {
                    decreaseButton.Click -= OnButtonClick;
                }
                decreaseButton = value;
                if (decreaseButton != null)
                {
                    decreaseButton.Click += OnButtonClick;
                }
            }
        }

        private ButtonBase increaseButton;
        private ButtonBase IncreaseButton
        {
            get
            {
                return increaseButton;
            }
            set
            {
                if (increaseButton != null)
                {
                    increaseButton.Click -= OnButtonClick;
                }
                
                increaseButton = value;

                if (increaseButton != null)
                {
                    increaseButton.Click += OnButtonClick;
                }
            }
        }

        public static readonly DependencyProperty ShowButtonSpinnerProperty = DependencyProperty.Register("ShowButtonSpinner", typeof(bool), typeof(ButtonSpinner), new UIPropertyMetadata(true));
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

        static ButtonSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonSpinner), new FrameworkPropertyMetadata(typeof(ButtonSpinner)));
        }

        public ButtonSpinner()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IncreaseButton = GetTemplateChild(PART_IncreaseButton) as ButtonBase;
            DecreaseButton = GetTemplateChild(PART_DecreaseButton) as ButtonBase;
            SetButtonUsage();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            Point mousePosition;
            if (IncreaseButton != null && IncreaseButton.IsEnabled == false)
            {
                mousePosition = e.GetPosition(IncreaseButton);
                if (mousePosition.X > 0 && mousePosition.X < IncreaseButton.ActualWidth &&
                    mousePosition.Y > 0 && mousePosition.Y < IncreaseButton.ActualHeight)
                {
                    e.Handled = true;
                }
            }
            if (DecreaseButton != null && DecreaseButton.IsEnabled == false)
            {
                mousePosition = e.GetPosition(DecreaseButton);
                if (mousePosition.X > 0 && mousePosition.X < DecreaseButton.ActualWidth &&
                    mousePosition.Y > 0 && mousePosition.Y < DecreaseButton.ActualHeight)
                {
                    e.Handled = true;
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    {
                        if (this.AllowSpin)
                        {
                            this.OnSpin(new SpinEventArgs(Spinner.SpinnerSpinEvent, SpinDirection.Increase));
                            e.Handled = true;
                        }
                        break;
                    }
                case Key.Down:
                    {
                        if (this.AllowSpin)
                        {
                            this.OnSpin(new SpinEventArgs(Spinner.SpinnerSpinEvent, SpinDirection.Decrease));
                            e.Handled = true;
                        }
                        break;
                    }
                case Key.Enter:
                    {
                        if (((this.IncreaseButton != null) && (this.IncreaseButton.IsFocused))
                          || ((this.DecreaseButton != null) && this.DecreaseButton.IsFocused))
                        {
                            e.Handled = true;
                        }
                        break;
                    }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!e.Handled && this.AllowSpin)
            {
                if (e.Delta != 0)
                {
                    var spinnerEventArgs = new SpinEventArgs(Spinner.SpinnerSpinEvent, (e.Delta < 0) ? SpinDirection.Decrease : SpinDirection.Increase, true);
                    this.OnSpin(spinnerEventArgs);
                    e.Handled = spinnerEventArgs.Handled;
                }
            }
        }

        protected override void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
        {
            SetButtonUsage();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (AllowSpin)
            {
                SpinDirection direction = sender == IncreaseButton ? SpinDirection.Increase : SpinDirection.Decrease;
                OnSpin(new SpinEventArgs(Spinner.SpinnerSpinEvent, direction));
            }
        }

        protected virtual void OnContentChanged(object oldValue, object newValue)
        {
        }

        protected virtual void OnAllowSpinChanged(bool oldValue, bool newValue)
        {
            SetButtonUsage();
        }

        private void SetButtonUsage()
        {
            if (IncreaseButton != null)
            {
                IncreaseButton.IsEnabled = AllowSpin && ((ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase);
            }
            if (DecreaseButton != null)
            {
                DecreaseButton.IsEnabled = AllowSpin && ((ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease);
            }
        }
    }
}
