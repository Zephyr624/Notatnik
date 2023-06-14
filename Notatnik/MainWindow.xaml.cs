using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Serialization;
using static System.Reflection.Metadata.BlobBuilder;


namespace Notatnik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();
        public MainWindow()
        {
            InitializeComponent();
            XmlSerializer xs = new(typeof(ObservableCollection<Note>));
            try
            {
                using StreamReader rd = new("notes.xml");
                Notes = xs.Deserialize(rd) as ObservableCollection<Note> ?? new ObservableCollection<Note>();
            }
            catch (Exception) { }

            NoteListBox.ItemsSource = Notes;
            SelectedNote = Notes.Count > 0 ? 0 : -1;
        }
        private ListCollectionView View
        {
            get
            {
                return (ListCollectionView)CollectionViewSource.GetDefaultView(Notes);
            }
        }
        public Note SelectedNoteItem { get; set; }
        public List<Note> FilteredNotes = new List<Note>();

        private int selectedNote;

        private DetailsWindow? detailsWindow = null;
        private string searchString { get; set; }
        private string FilterItem { get; set; }
        private DateTime? dateFrom { get; set; }
        private DateTime? dateTo { get; set; }


        public int SelectedNote
        {
            get { return selectedNote; }
            set
            {
                selectedNote = value;
                OnPropertyChanged("SelectedNote");
                OnPropertyChanged("ItemSelected");
                NotifyDetails();
            }
        }
        public string SearchString
        {
            get { return searchString; }
            set
            {
                searchString = value;
                OnPropertyChanged("SearchString");
            }
        }
        public DateTime? DateTo
        {
            get { return dateTo; }
            set
            {
                dateTo = value;
                OnPropertyChanged("DateFrom");
            }
        }
        public DateTime? DateFrom
        {
            get { return dateFrom; }
            set
            {
                dateFrom = value;
                OnPropertyChanged("DateFrom");
            }
        }
        public bool ItemSelected { get { return SelectedNote != -1; } }

        private void NotifyDetails()
        {
            if (detailsWindow != null)
            {
                detailsWindow.Note = SelectedNoteItem;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        private void AddClick(object sender, RoutedEventArgs e)
        {
            var addWindow = new EditWindow();
            addWindow.Note.NoteDate = DateTime.Today;
            if (addWindow.ShowDialog() == true)
            {
                Notes.Add(addWindow.Note);
                SelectedNote = 0;
            }
        }
        private void EditClick(object sender, RoutedEventArgs e)
        {
            var note = SelectedNoteItem;
            var editWindow = new EditWindow();
            editWindow.Note.Title = note.Title;
            editWindow.Note.NoteDate = note.NoteDate;
            editWindow.Note.Contents = note.Contents;
            if (editWindow.ShowDialog() == true)
            {
                note.Title = editWindow.Note.Title;
                note.NoteDate = editWindow.Note.NoteDate;
                note.Contents = editWindow.Note.Contents;
                NoteListBox.Items.Refresh();
                NotifyDetails();
            }
        }
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz usunąć ten film?", "Usuń", MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            Notes.Remove(SelectedNoteItem);
            selectedNote = Notes.Count > 0 ? Notes.Count : -1;
        }
        private void DetailsClick(object sender, RoutedEventArgs e)
        {

            detailsWindow = new DetailsWindow();
            NotifyDetails();
            detailsWindow.Show();
            DetailsButton.IsEnabled = false;
            detailsWindow.Closed += (ss, ee) =>
            {
                detailsWindow = null;
                if (Notes.Count != 0)
                { DetailsButton.IsEnabled = true; }
            };
        }
        private void SearchClick(object sender, RoutedEventArgs e)
        {
            //FilteredNotes=Notes.Where(w => w.Title.Contains(searchString)).ToList();
            //NoteListBox.ItemsSource = FilteredNotes;
            //NoteListBox.Items.Refresh();
            CollectionViewSource.GetDefaultView(NoteListBox.ItemsSource).Refresh();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Note>));
            using (StreamWriter wr = new StreamWriter("notes.xml"))
            {
                xs.Serialize(wr, Notes);
            }
            base.OnClosing(e);
            detailsWindow?.Close();
        }

        private void ReloadWindow(object sender, RoutedEventArgs e)
        {
            SelectedNote = Notes.Count > 0 ? 0 : -1;
            SearchString = "";
            Filter_Combobox.SelectedIndex = 0;
            DateTo = null;
            DateFrom = null;
            CollectionViewSource.GetDefaultView(NoteListBox.ItemsSource).Refresh();
        }
        private void ClearClick(object sender, RoutedEventArgs e)
        {
            ReloadWindow(sender, e);
        }

        private void Filter(object sender, RoutedEventArgs e)
        {
            View.Filter = delegate (object item)
            {
                bool showNote = true;
                if (item is not Note note) { return false; }

                if (!string.IsNullOrEmpty(searchString))
                {
                    showNote = note.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase);
                }

                if (DateFrom.HasValue)
                {
                    showNote = showNote && note.NoteDate.CompareTo(DateFrom.Value) >= 0;
                }

                if (DateTo.HasValue)
                {
                    showNote = showNote && note.NoteDate.CompareTo(DateTo.Value) <= 0;
                }
                return showNote;
            };
        }

        private void SortTitle(object sender, RoutedEventArgs e)
        {
            View.SortDescriptions.Clear();
            View.SortDescriptions.Add(new SortDescription(nameof(Note.Title), ListSortDirection.Ascending));
        }

        private void SortTitleDesc(object sender, RoutedEventArgs e)
        {
            View.SortDescriptions.Clear();
            View.SortDescriptions.Add(new SortDescription(nameof(Note.Title), ListSortDirection.Descending));
        }

        private void SortDate(object sender, RoutedEventArgs e)
        {
            View.SortDescriptions.Clear();
            View.SortDescriptions.Add(new SortDescription(nameof(Note.NoteDate), ListSortDirection.Ascending));
        }

        private void SortDateDesc(object sender, RoutedEventArgs e)
        {
            View.SortDescriptions.Clear();
            View.SortDescriptions.Add(new SortDescription(nameof(Note.NoteDate), ListSortDirection.Descending));
        }
    }
}
