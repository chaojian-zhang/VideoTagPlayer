using LibVLCSharp.Shared;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoTagPlayer.Models;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace VideoTagPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Components
        LibVLC _LibVLC;
        MediaPlayer _MediaPlayer;
        #endregion

        #region Construction
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        public VideoNote Note { get; set; }
        #endregion

        #region View Properties
        private TimeSpan _CurrentTime;
        public TimeSpan CurrentTime { get => _CurrentTime; set => SetField(ref _CurrentTime, value); }
        private new NoteTag Tag { get; set; }
        private NotesWindow NotesWindow { get; set; }
        #endregion

        #region Events
        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();
            
            // Initialize components
            _LibVLC = new LibVLC();
            _MediaPlayer = new MediaPlayer(_LibVLC);

            // Setup media player
            VideoView.MediaPlayer = _MediaPlayer;
            _MediaPlayer.TimeChanged += _MediaPlayer_TimeChanged;

            // Show additional panels
            NotesWindow = new NotesWindow() { Owner = this };
            NotesWindow.Show();
        }

        private void _MediaPlayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            CurrentTime = TimeSpan.FromSeconds(_MediaPlayer.Time / 1000);
            if(Note != null && Note.GetNoteAt(CurrentTime) != null)
            {
                Tag = Note.GetNoteAt(CurrentTime);
                Dispatcher.Invoke(() => OpenTagButton.Visibility = Visibility.Visible);
            }
            else
            {
                Dispatcher.Invoke(() => OpenTagButton.Visibility = Visibility.Collapsed);
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_MediaPlayer.IsPlaying)
                _MediaPlayer.Stop();
            if(Note!=null)
                Note.Save();
        }
        private void OpenTagButton_Click(object sender, RoutedEventArgs e)
        {
            AddNoteWindow noteWindow = new AddNoteWindow(Note, Tag);
            noteWindow.Owner = this;
            noteWindow.Show();
        }
        #endregion

        #region Commands
        private void OpenFileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void OpenFileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == true)
            {
                // Initialize note
                Note = new VideoNote(dialog.FileName);
                // Start play
                IntroTextBlock.Visibility = Visibility.Collapsed;
                _MediaPlayer.Play(new Media(_LibVLC, new Uri(dialog.FileName)));
                // Update view
                NotesWindow.Tags = Note.Tags;
            }
        }
        private void PlayPauseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;

        private void PlayPauseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_MediaPlayer.IsPlaying)
                _MediaPlayer.Pause();
            else if(_MediaPlayer.WillPlay)
                _MediaPlayer.Play();
        }

        private void AddNoteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = Note != null;
        private void AddNoteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(_MediaPlayer.WillPlay)
            {
                _MediaPlayer.Pause();

                AddNoteWindow noteWindow = new AddNoteWindow(Note, TimeSpan.FromSeconds(_MediaPlayer.Time / 1000));
                noteWindow.Owner = this;
                noteWindow.Show();
                noteWindow.Closed += (o, v) => 
                { 
                    _MediaPlayer.Play();
                    // Update view
                    NotesWindow.Tags = Note.Tags;
                };
            }
        }
        private void ForwardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = _MediaPlayer.WillPlay;

        private void ForwardCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _MediaPlayer.Time += 10*1000;
        }

        private void BackwardCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _MediaPlayer.Time -= 5 * 1000;
        }

        private void BackwardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        => e.CanExecute = _MediaPlayer.WillPlay;
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
    }
}
