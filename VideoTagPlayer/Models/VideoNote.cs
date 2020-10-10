using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace VideoTagPlayer.Models
{
    public class NoteTag
    {
        public TimeSpan Location { get; set; }
        public string Content { get; set; }
    }

    public class VideoNote
    {
        #region Properties
        public string FilePath { get; set; }
        [YamlIgnore]
        public string NoteName => Path.GetFileNameWithoutExtension(FilePath) + " (Note).yaml";
        [YamlIgnore]
        public string NotePath => Path.Combine(Path.GetDirectoryName(FilePath), NoteName);
        public ObservableCollection<NoteTag> Tags { get; set; }
        #endregion

        #region Construction
        public VideoNote() { }
        public VideoNote(string filePath)
        {
            FilePath = filePath;
            Tags = new ObservableCollection<NoteTag>();
            // Try load
            if(File.Exists(NotePath))
            {
                Tags = new ObservableCollection<NoteTag>(Load(NotePath).Tags.OrderBy(t => t.Location)); // Sort
            }
                
        }
        #endregion

        #region Methods
        public void AddNoteAt(TimeSpan location, string content)
        {
            var tag = GetNoteAt(location);
            if (tag == null)
            {
                Tags.Add(new NoteTag { Location = location, Content = content });
                // Sort
                Tags = new ObservableCollection<NoteTag>(Tags.OrderBy(t => t.Location));
            }
            else
                tag.Content = content;
        }
        public NoteTag GetNoteAt(TimeSpan location)
        {
            return Tags.SingleOrDefault(t => t.Location == location);
        }
        public void RemoveNote(NoteTag note)
        {
            Tags.Remove(note);
        }
        public void Save()
        {
            string yaml = new Serializer().Serialize(this);
            File.WriteAllText(NotePath, yaml);
        }
        public static VideoNote Load(string notePath)
        {
            if (!File.Exists(notePath))
                throw new ArgumentException("Note path doesn't exist!");
            return new Deserializer().Deserialize<VideoNote>(File.ReadAllText(notePath));
        }

        internal string GetScreenshotFor(long timeInSeconds)
        {
            string imageName = $"{Path.GetFileNameWithoutExtension(NotePath)}-{Convert.ToString(timeInSeconds)}.png";
            return Path.Combine(Path.GetDirectoryName(FilePath), imageName);
        }
        #endregion
    }
}
