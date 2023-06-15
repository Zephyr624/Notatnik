using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Xml.Serialization;


namespace Notatnik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();
        public ObservableCollection<Category> Categories { get; set; }
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
            Categories = Category.GetCategoriesFromFile();
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
        private Note selectedNoteItem;
        public Note SelectedNoteItem
        {
            get => selectedNoteItem;
            set
            {
                selectedNoteItem = value;
                NotifyDetails();
            }
        }
        public List<Note> FilteredNotes = new();

        private int selectedNote;

        private DetailsWindow? detailsWindow = null;
        private string searchString { get; set; }
        private DateTime? dateFrom { get; set; }
        private DateTime? dateTo { get; set; }


        public int SelectedNote
        {
            get { return selectedNote; }
            set
            {
                selectedNote = value;
                OnPropertyChanged("ItemSelected");

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
            var addWindow = new EditWindow(Categories);
            addWindow.Note.NoteDate = DateTime.Today;
            if (addWindow.ShowDialog() == true)
            {
                Notes.Add(addWindow.Note);
                SelectedNote = 0;
            }
        }
        private void EditClick(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditWindow(Categories, SelectedNoteItem);
            if (editWindow.ShowDialog() == true)
            {
                NoteListBox.Items.Refresh();
                NotifyDetails();
                CollectionViewSource.GetDefaultView(NoteListBox.ItemsSource).Refresh();
            }
        }
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz usunąć tę notatkę?", "Usuń", MessageBoxButton.YesNo,
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
            CollectionViewSource.GetDefaultView(NoteListBox.ItemsSource).Refresh();
        }

        private void CategoryClick(object sender, RoutedEventArgs e)
        {
            var categoryWindow = new CategoryWindow(Categories);
            categoryWindow.ShowDialog();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            XmlSerializer xs = new(typeof(ObservableCollection<Note>));
            using (StreamWriter wr = new("notes.xml"))
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
