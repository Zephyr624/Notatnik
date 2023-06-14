using System;

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

        public override string ToString()
        {
            return title;
        }
    }
}
