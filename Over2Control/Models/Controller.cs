using System.Collections.Generic;
using System.ComponentModel;

namespace Over2Control.Models
{
    public class Controller : INotifyPropertyChanged
    {
        private string _title;
        private string _id;

        public List<Channel> Channels { get; set; } = new List<Channel>();

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("ID");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
