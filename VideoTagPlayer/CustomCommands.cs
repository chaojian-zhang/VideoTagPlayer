using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoTagPlayer
{
    public static class CustomCommands
    {
        #region UI Commands
        public static readonly RoutedUICommand OpenFile = new RoutedUICommand("Open file", "OpenFile", typeof(CustomCommands));
        public static readonly RoutedUICommand PlayPause = new RoutedUICommand("Play or pause", "PlayPause", typeof(CustomCommands));
        public static readonly RoutedUICommand AddNote = new RoutedUICommand("Add note at current time", "AddNote", typeof(CustomCommands));
        public static readonly RoutedUICommand Forward = new RoutedUICommand("Forward 10 seconds", "Forward", typeof(CustomCommands));
        public static readonly RoutedUICommand Backward = new RoutedUICommand("Backward 5 seconds", "Backward", typeof(CustomCommands));
        #endregion
    }
}
