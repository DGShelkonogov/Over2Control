using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Over2Control.Models
{
    public class MenuItem : INotifyPropertyChanged
    {
        public MenuItem()
        {
            Items = new ObservableCollection<MenuItem>();
        }

        private string _title;

        private bool _isChecked;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public ObservableCollection<MenuItem> Items { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
