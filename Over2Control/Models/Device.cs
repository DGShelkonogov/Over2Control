using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Over2Control.Models
{
    public class Device : INotifyPropertyChanged
    {
        public static List<Controller> ControllerIsUsed { get; set; } = new List<Controller>();
        public static ObservableCollection<Controller> Controllers { get; set; } = new ObservableCollection<Controller>();





        private string _ip;
        public string Ip
        {
            get { return _ip; }
            set
            {
                _ip = value;
                OnPropertyChanged("Ip");
            }
        }

        private Controller _controller;
        public Controller SelectedController
        {
            get { return _controller; }
            set
            {
                if (value != null)
                {
                    if (!ControllerIsUsed.Exists(x => x == value))
                        ControllerIsUsed.Add(value);
                    else
                    {
                        MessageBox.Show("Такой контроллер уже используется");
                        return;
                    }
                }
                _controller = value;
                OnPropertyChanged("SelectedController");
            }
        }


        private string _port;
        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        private int _timeout;
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                if(value < 0)
                    _timeout = 1000;
                else
                    _timeout = value;
                OnPropertyChanged("Timeout");
            }
        }

        private int _hostNum;
        public int HostNum
        {
            get { return _hostNum; }
            set
            {
                if(value < 1 || value > 4)
                {
                    _hostNum = 4;
                }else
                    _hostNum = value;
                OnPropertyChanged("HostNum");
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
