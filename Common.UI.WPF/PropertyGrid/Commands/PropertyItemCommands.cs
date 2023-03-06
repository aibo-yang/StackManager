using System.Windows.Input;

namespace Common.UI.WPF.PropertyGrid.Commands
{
    public static class PropertyItemCommands
    {
        private static RoutedCommand resetValueCommand = new RoutedCommand();
        public static RoutedCommand ResetValue
        {
            get
            {
                return resetValueCommand;
            }
        }
    }
}
