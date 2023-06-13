using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;


namespace Notatnik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Note> Notes
        { get; set; } = new ObservableCollection<Note>();
        public MainWindow()
        {
            InitializeComponent();
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Note>));
            using (StreamReader rd = new StreamReader("notes.xml"))
            {
                //if (xDoc.ChildNodes.Count > 1)
                //{
                    Notes = xs.Deserialize(rd) as ObservableCollection<Note>;
                //}
            }
        }
        

        public List<Note> FilteredNotes= new List<Note>();

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
        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            SelectedNote = 0;
            NoteListBox.ItemsSource = Notes;
        }
        private void NotifyDetails()
        {
            if (detailsWindow != null)
            {
                if (FilteredNotes.Count>SelectedNote)
                {
                    var filterednote = FilteredNotes[SelectedNote];
                    if (searchString != null || FilterItem != null || (DateFrom != null && DateTo != null))
                    {
                        detailsWindow.Note = filterednote;
                    }
                }else
                {
                    var unfilterednote = Notes[SelectedNote];
                    detailsWindow.Note = unfilterednote;
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        private void AddClick(object sender, RoutedEventArgs e)
        {
            var addWindow = new EditWindow();
            addWindow.Note.NoteDate = DateTime.Today;
            if (addWindow.ShowDialog() == true)
            {
                Notes.Add(addWindow.Note);
                SelectedNote = 0;
                DetailsButton.IsEnabled = true;
            }
        }
        private void EditClick(object sender, RoutedEventArgs e)
        {
            var filterednote = FilteredNotes[SelectedNote];
            var unfilterednote = Notes[SelectedNote];
            var note = unfilterednote;
            if (searchString != null || FilterItem != null || (DateFrom != null && DateTo != null))
            {
                note= filterednote;
            }
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
            if (searchString != null || FilterItem != null || (DateFrom != null && DateTo != null))
            {
                var filterednote = FilteredNotes[SelectedNote];
                FilteredNotes.RemoveAt(SelectedNote);
                NoteListBox.Items.Refresh();
                selectedNote =Notes.IndexOf(filterednote);
            }

            Notes.RemoveAt(SelectedNote);
            NoteListBox.Items.Refresh();
            if (Notes.Count != 0)
            {
                DetailsButton.IsEnabled = true;
                SelectedNote = 0;
            }
            else { DetailsButton.IsEnabled = false; SelectedNote = -1; }
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
            FilteredNotes=Notes.Where(w => w.Title.Contains(searchString)).ToList();
            NoteListBox.ItemsSource = FilteredNotes;
            NoteListBox.Items.Refresh();
        }
        private void SortClick(object sender, RoutedEventArgs e)
        {
            FilteredNotes = Notes.ToList();
            switch (FilterItem)
            {
                case "Data malejąco":
                    FilteredNotes = Notes.OrderByDescending(w => w.NoteDate).ToList();
                    break;
                case "Data rosnąco":
                    FilteredNotes = Notes.OrderBy(w => w.NoteDate).ToList();
                    break;
                case "Tytuł A-Z":
                    FilteredNotes = Notes.OrderBy(w => w.Title).ToList();
                    break;
                case "Tytuł Z-A":
                    FilteredNotes = Notes.OrderByDescending(w => w.Title).ToList();
                    break;
                default:
                    FilteredNotes = Notes.ToList();
                    break;

            }
            NoteListBox.ItemsSource = FilteredNotes;
            NoteListBox.Items.Refresh();
        }
        private void OptionClick(object sender, RoutedEventArgs e)
        {
            if (Filter_Combobox.SelectedItem != null)
            {
                var item = (ComboBoxItem)Filter_Combobox.SelectedItem;
                FilterItem=item.Content.ToString();
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Note>));
            using (StreamWriter wr = new StreamWriter("notes.xml"))
            {
                xs.Serialize(wr, Notes);
            }
            base.OnClosing(e);
            if (detailsWindow != null)
                detailsWindow.Close();
        }

        private void FilterClick(object sender, RoutedEventArgs e)
        {
            FilteredNotes = Notes.Where(w => w.NoteDate.CompareTo(DateFrom)>0 && w.NoteDate.CompareTo(DateTo)<0).ToList();
            NoteListBox.ItemsSource = FilteredNotes;
            NoteListBox.Items.Refresh();
        }
        private void ReloadWindow(object sender, RoutedEventArgs e)
        {
            SelectedNote = -1;
            NoteListBox.ItemsSource = Notes;
            searchString = null;
            FilterItem = null;
            DateTo = null;
            DateFrom = null;
        }
        private void ClearClick(object sender, RoutedEventArgs e)
        {
            ReloadWindow(sender, e);
        }
    }
}
