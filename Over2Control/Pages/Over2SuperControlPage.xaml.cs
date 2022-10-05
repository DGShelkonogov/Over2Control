using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Over2Control.Models;
using Over2Control.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Over2Control.Pages
{

    public partial class Over2SuperControlPage : Page
    {
        private ObservableCollection<Device> _items = new ObservableCollection<Device>();

        private Device DataGridItemTestBuffer;


        public Over2SuperControlPage()
        {
            InitializeComponent();

            UpdateListDevices();

            MainDataGrid.CanUserAddRows = false;
            MainDataGrid.ItemsSource = _items;
        }



        private void ButtonClickSave(object sender, RoutedEventArgs e)
        {
            SaveChange();
        }

        public void SaveChange()
        {
            try
            {
                string json = File.ReadAllText(MainWindow.PathToAppsettings);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                jsonObj["WorkerOptions"]["Controllers"] = new JObject();
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(MainWindow.PathToAppsettings, output);


                foreach (var item in _items)
                {
                    AddDevice(item);
                }
            }
            catch (Exception e)
            {

            }
        }

        public void RemoveDevice(string deviceId)
        {
            try
            {
                string json =  File.ReadAllText(MainWindow.PathToAppsettings);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                jsonObj["WorkerOptions"]["Controllers"].Remove(deviceId);
                string output = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(MainWindow.PathToAppsettings, output);
            }
            catch (Exception e)
            {

            }
        }

        public void AddDevice(Device item)
        {
            try
            {
                string json = File.ReadAllText(MainWindow.PathToAppsettings);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);

                var Controllers = jsonObj["WorkerOptions"]["Controllers"];
                Controllers.Add(item.SelectedController.ID, new JObject());

                Controllers[item.SelectedController.ID].Add("Name", item.SelectedController.Title);
                Controllers[item.SelectedController.ID].Add("id_dev", item.SelectedController.ID);
                Controllers[item.SelectedController.ID].Add("Ip", item.Ip);
                Controllers[item.SelectedController.ID].Add("Port", Convert.ToInt32(item.Port));
                Controllers[item.SelectedController.ID].Add("IsActive", item.IsActive);
                Controllers[item.SelectedController.ID].Add("timeout_DB_answer", item.Timeout);
                Controllers[item.SelectedController.ID].Add("Host_num", item.HostNum);
                Controllers[item.SelectedController.ID].Add("Channels", new JObject());

                var ControllerChannels = Controllers[item.SelectedController.ID]["Channels"];
                foreach (var channel in item.SelectedController.Channels)
                    ControllerChannels[channel.Number] = channel.Debice;



                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(MainWindow.PathToAppsettings, output);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public async void UpdateListDevices()
        {
            var connection = await DBConnection.GetFbConnection();
            if (connection != null)
            {
                connection.Open();

                _items.Clear();

                var command = new FbCommand("select d.id_dev, cast(d.name as varchar(50) character set UTF8) as name from device d " +
                    "where d.id_reader is null and d.\"ACTIVE\">0 and d.id_devtype in (1,2)", connection);

                string json = File.ReadAllText(MainWindow.PathToAppsettings);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);

                var controllers = jsonObj["WorkerOptions"]["Controllers"];

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["NAME"].ToString();
                        string id_dev = reader["ID_DEV"].ToString();

                        var controller = new Controller()
                        {
                            Title = name,
                            ID = id_dev
                        };

                        var commandSelectChannels =
                               new FbCommand($"select d.id_dev as id_device, " +
                               $"d2.id_reader as chanel_number, " +
                               $"d2.id_dev as debice_chanel " +
                               $"from device d join device d2 on d2.id_ctrl=d.id_ctrl and d2.id_reader " +
                               $"is not null where d.id_reader is null and d.id_dev = {id_dev}", connection);
                        using (var readerSelectChannels = commandSelectChannels.ExecuteReader())
                        {
                            while (readerSelectChannels.Read())
                            {
                                string number = readerSelectChannels["chanel_number"].ToString();
                                string debice = readerSelectChannels["debice_chanel"].ToString();

                                controller.Channels.Add(new Channel() { Debice = debice, Number = number });
                            }
                        }

                        Device.Controllers.Add(controller);

                        var obj = controllers.GetValue(id_dev);

                        if (obj != null)
                        {
                            var device = new Device()
                            {
                                Ip = obj.Ip,
                                IsActive = obj.IsActive,
                                Port = obj.Port,
                                HostNum = obj.Host_num,
                                Timeout = obj.timeout_DB_answer,
                                SelectedController = controller
                            };

                            _items.Add(device);
                        }
                    }
                }
                connection.Close();
            }
        }


        private void ButtonClickAdd(object sender, RoutedEventArgs e)
        {
            DataGridItemTestBuffer = new Device()
            {
                Port = "8192",
                IsActive = true,
                HostNum = 4,
                Timeout = 1000
            };
            _items.Add(DataGridItemTestBuffer);
        }

        private void ButtonClickRemove(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Device o = (sender as Button).Tag as Device;
                if (o != null)
                {
                    _items.Remove(o);
                    Device.ControllerIsUsed.Remove(o.SelectedController);
                }
            }
        }

        private void ButtonClickUpdateControllers(object sender, RoutedEventArgs e)
        {
            UpdateListDevices();
        }
    }
}
