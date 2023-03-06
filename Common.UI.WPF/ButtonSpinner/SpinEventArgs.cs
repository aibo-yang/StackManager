﻿using System.Windows;

namespace Common.UI.WPF
{
    public class SpinEventArgs : RoutedEventArgs
    {
        public SpinDirection Direction
        {
            get;
            private set;
        }

        public bool UsingMouseWheel
        {
            get;
            private set;
        }

        public SpinEventArgs(SpinDirection direction)
          : base()
        {
            Direction = direction;
        }

        public SpinEventArgs(RoutedEvent routedEvent, SpinDirection direction)
          : base(routedEvent)
        {
            Direction = direction;
        }

        public SpinEventArgs(SpinDirection direction, bool usingMouseWheel)
          : base()
        {
            Direction = direction;
            UsingMouseWheel = usingMouseWheel;
        }

        public SpinEventArgs(RoutedEvent routedEvent, SpinDirection direction, bool usingMouseWheel)
          : base(routedEvent)
        {
            Direction = direction;
            UsingMouseWheel = usingMouseWheel;
        }
    }
}
