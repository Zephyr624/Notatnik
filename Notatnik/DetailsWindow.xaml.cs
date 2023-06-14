using System.ComponentModel;
using System.Windows;

namespace Notatnik
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window, INotifyPropertyChanged
    {
        public DetailsWindow()
        {
            InitializeComponent();
        }
        private Note note = new Note();
        public Note Note
        {
            get { return note; }
            set
            {
                note.Title = value.Title;
                note.NoteDate = value.NoteDate;
                DateToString = note.NoteDate.ToString("D");
                note.Contents = value.Contents;
                NotifyPropertyChanged(nameof(Note));
            }
        }
        private string? noteDate;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string DateToString
        {
            get { return noteDate; }
            private set { noteDate = value; }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
