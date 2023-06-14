using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Xml.Serialization;

namespace Notatnik
{
    /// <summary>
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        private Category? selectedCategory;
        public Category? SelectedCategory
        {
            get => selectedCategory; set
            {
                selectedCategory = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }
        public bool Enabled { get => SelectedCategory != null; }
        public CategoryWindow()
        {
            InitializeComponent();
            XmlSerializer xs = new(typeof(ObservableCollection<Category>));
            try
            {
                using StreamReader rd = new("categories.xml");
                Categories = xs.Deserialize(rd) as ObservableCollection<Category> ?? new ObservableCollection<Category>();
            }
            catch (Exception) { }
            SelectedCategory = Categories.Count > 0 ? Categories[0] : null;
            CategoryListBox.ItemsSource = Categories;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            var addWindow = new CategoryEditWindow();
            if (addWindow.ShowDialog() == true)
            {
                Categories.Add(addWindow.Category);
            }
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            var editWindow = new CategoryEditWindow()
            {
                Category = SelectedCategory!
            };
            if (editWindow.ShowDialog() == true)
            {
                CategoryListBox.Items.Refresh();
            }
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz usunąć tą kategorie?", "Usuń", MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            Categories.Remove(SelectedCategory!);

            SelectedCategory = Categories.Count > 0 ? Categories[0] : null;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            XmlSerializer xs = new(typeof(ObservableCollection<Category>));
            using (StreamWriter wr = new("categories.xml"))
            {
                xs.Serialize(wr, Categories);
            }
            base.OnClosing(e);
        }
    }
}
