using FirebirdSql.Data.FirebirdClient;
using Over2Control.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MenuItem = Over2Control.Models.MenuItem;

namespace Over2Control.Pages
{
    public partial class ConfigOver2Page : Page
    {

        private Dictionary<string, string> _config = new Dictionary<string, string>();

        public ConfigOver2Page()
        {
            InitializeComponent();
            //UpdateListDevices();
        }

        public async void UpdateListDevices()
        {
            var connection = await DBConnection.GetFbConnection();
            if(connection != null)
            {
                connection.Open();

                _config.Clear();

                var command = new FbCommand("select * from devgroup dg where dg.id_parent=1", connection);

                string json = File.ReadAllText(MainWindow.PathToAppsettings);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                
                var devicesGroup = jsonObj["WorkerOptions"]["DeviceGroup"];
                
                var root = new MenuItem() { Title = "Устройства", IsChecked = true };

                using (FbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["NAME"].ToString();
                        string deviceGroup = reader["ID_DEVGROUP"].ToString();

                        var obj = devicesGroup.GetValue(deviceGroup);

                        bool isChecked = (obj != null);

                        root.Items.Add(new MenuItem() { IsChecked = isChecked, Title = name });

                        if (!isChecked && root.IsChecked)
                        {
                            root.IsChecked = false;
                        }

                        _config.Add(name, deviceGroup);
                    }
                }

                trvMenu.Items.Clear();
                trvMenu.Items.Add(root);

                connection.Close();
            }
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var ch = sender as CheckBox;
            if (ch != null)
            {
                foreach (MenuItem item in trvMenu.Items)
                {
                    FindItem(item, ch.Content.ToString(), true);
                }
            }
        }

        private void CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            var ch = sender as CheckBox;
            if (ch != null)
            {
                foreach (MenuItem item in trvMenu.Items)
                    FindItem(item, ch.Content.ToString(), false);
            }
        }

        public void FindItem(MenuItem item, string name, bool check)
        {
            foreach (var i in item.Items)
            {
                FindItem(i, name, check);
            }

            if (name == item.Title)
            {
                CheckedAll(item, check);
            }
        }

        public void CheckedAll(MenuItem item, bool check)
        {
            foreach (var i in item.Items)
            {
                CheckedAll(i, check);
            }
            item.IsChecked = check;
        }

        public void AddDevice(string deviceGroup, string name)
        {
            try
            {
                string json = File.ReadAllText(MainWindow.PathToAppsettings);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["WorkerOptions"]["DeviceGroup"].Add(deviceGroup, name);
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(MainWindow.PathToAppsettings, output);
            }
            catch(Exception e)
            {
               
            }
        }

        public void RemoveDevice(string deviceGroup)
        {
            try
            {
                string json = File.ReadAllText(MainWindow.PathToAppsettings);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["WorkerOptions"]["DeviceGroup"].Remove(deviceGroup);
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(MainWindow.PathToAppsettings, output);
            }
            catch(Exception e)
            {
               
            }
        }

        private void ButtonClickSave(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem item in trvMenu.Items)
                SaveChange(item.Items);
        }
        
        private void ButtonClickUpdate(object sender, RoutedEventArgs e)
        {
            UpdateListDevices();
        }

        public void SaveChange(ObservableCollection<MenuItem> menuItems)
        {
            foreach (MenuItem item in menuItems)
            {
                if (item.IsChecked)
                {
                    var deviceGroup = _config[item.Title];
                    AddDevice(deviceGroup, item.Title);
                }
                else
                    RemoveDevice(_config[item.Title]);
            }
        }
    }
}
