using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VideoTagPlayer.Models;

namespace VideoTagPlayer
{
    /// <summary>
    /// Interaction logic for AddNoteWindow.xaml
    /// </summary>
    public partial class AddNoteWindow : Window, INotifyPropertyChanged
    {
        #region Properties
        public AddNoteWindow(VideoNote note, TimeSpan location)
        {
            Note = note;
            Location = location;
            InitializeComponent();
        }
        public AddNoteWindow(VideoNote note, NoteTag tag)
        {
            Note = note;
            Location = tag.Location;
            TagContent = tag.Content;
            InitializeComponent();
        }
        private VideoNote Note;
        #endregion

        #region View 
        private string _Screenshot;
        public string Screenshot { get => _Screenshot; set => SetField(ref _Screenshot, value); }
        private TimeSpan _Location;
        public TimeSpan Location { get => _Location; set => SetField(ref _Location, value); }
        private string _TagContent;
        public string TagContent { get => _TagContent; set => SetField(ref _TagContent, value); }
        #endregion

        #region Data Binding
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<type>(ref type field, type value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<type>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Screenshot = Note.GetScreenshotFor((long)Location.TotalSeconds);
        }
        // TODO: There is a bug here - open a video, use F2 to create a new tag, don't enter anything and directly click "Remove". The application will crash.
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove tag
            var tag = Note.GetNoteAt(Location);
            if(tag != null)
                Note.RemoveNote(tag);
            // Remove screenshot
            string screenshot = Note.GetScreenshotFor((long)Location.TotalSeconds);
            if (File.Exists(screenshot))
                File.Delete(screenshot);
            // Close window
            this.Close();
        }
        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            Note.AddNoteAt(Location, TagContent);
            this.Close();
        }
        #endregion
    }
}
