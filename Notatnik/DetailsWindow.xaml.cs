using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Notatnik
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
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
            }
        }
        private string? noteDate;
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
