using System.Windows.Input;

namespace Common.UI.WPF.PropertyGrid.Commands
{
    public class PropertyGridCommands
    {
        private static RoutedCommand clearFilterCommand = new RoutedCommand();
        public static RoutedCommand ClearFilter
        {
            get
            {
                return clearFilterCommand;
            }
        }
    }
}
