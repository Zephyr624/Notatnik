using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Notatnik
{
    public class Note
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private DateTime noteDate;
        public DateTime NoteDate
        {
            get { return noteDate; }
            set { noteDate = value; }
        }
        private string contents;
        public string Contents
        {
            get { return contents; }
            set { contents = value; }
        }
        public Category? Category { get; set; }

        public override string ToString()
        {
            return title;
        }
    }

    //[ValueConversion(typeof(Category), typeof(string))]
    public class CategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return "Brak";
            }
            else
            {
                var cat = value as Category;
                return cat?.Name ?? "Brak";
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = (string)value;
            return new Category()
            {
                Name = name
            };
        }
    }
}
