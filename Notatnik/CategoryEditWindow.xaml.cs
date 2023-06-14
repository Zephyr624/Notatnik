using System.Windows;

namespace Notatnik
{
    /// <summary>
    /// Interaction logic for CategoryEditWindow.xaml
    /// </summary>
    public partial class CategoryEditWindow : Window
    {
        public CategoryEditWindow()
        {
            InitializeComponent();
        }
        public Category Category{ get; set; } = new Category();

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
