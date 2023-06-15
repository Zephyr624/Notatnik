using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Notatnik
{
    public class Category
    {
        public string Name { get; set; }

        public static ObservableCollection<Category> GetCategoriesFromFile()
        {
            var categories = new ObservableCollection<Category>();
            XmlSerializer xs = new(typeof(ObservableCollection<Category>));
            try
            {
                using StreamReader rd = new("categories.xml");
                categories = xs.Deserialize(rd) as ObservableCollection<Category> ?? new ObservableCollection<Category>();
            }
            catch (Exception) { }
            return categories;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
