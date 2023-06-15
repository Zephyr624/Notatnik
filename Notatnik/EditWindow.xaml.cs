using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Notatnik
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public ObservableCollection<Category> Categories { get; set; }
        public EditWindow()
        {
            InitializeComponent();
        }
        public EditWindow(ObservableCollection<Category> categories, Note note = null) : this()
        {
            Categories = categories;
            CategoryComboBox.ItemsSource = categories;
            if (note != null)
            {
                CategoryComboBox.SelectedItem = categories.FirstOrDefault(c => c.Name == note.Category?.Name);
                Note = note;
            }
        }
        public Note Note { get; set; } = new Note();

        private void OkClick(object sender, RoutedEventArgs e)
        {
            Note.Category = (Category)CategoryComboBox.SelectedItem;

            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
