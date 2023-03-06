using System;

namespace Common.UI.WPF.PropertyGrid
{
    internal struct FilterInfo
    {
        public string InputString { get; set; }
        public Predicate<object> Predicate { get; set; }
    }
}
