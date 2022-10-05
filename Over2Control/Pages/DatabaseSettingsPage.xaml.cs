using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using Newtonsoft.Json;
using Over2Control.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Over2Control.Pages
{
    public enum ServerType
    {
        LOCAL,
        REMOTE
    }

    public partial class DatabaseSettingsPage : Page
    {

        private ObservableCollection<ServerType> _serverTypes = new ObservableCollection<ServerType>()
        {
            ServerType.LOCAL,
            ServerType.REMOTE
        };

        public DatabaseSettingsPage()
        {
            try
            {
                InitializeComponent();

                cmbServerType.ItemsSource = _serverTypes;
                txtPort.Text = "3050";

                FillFields();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка произошла на странице 'База данных'. Ошибка: {e.Message}");
            }
        }

        private void ButtonClickOpenFile(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "База данных (*.gdb)|*.gdb";
            if (openFileDialog.ShowDialog() == true)
                txtPath.Text = openFileDialog.FileName;
        }

        private async void ButtonClickCheckConnection(object sender, RoutedEventArgs e)
        {
            try
            {
                var connectionStringBuilder = new FbConnectionStringBuilder
                {
                    UserID = txtUsername.Text,
                    Password = txtPassword.Password,
                    Database = txtPath.Text,
                    DataSource = txtAddress.Text,
                    Port = Int32.Parse(txtPort.Text),
                    ServerType = 0
                };

                await Task.Run(() => {
                    var connection = new FbConnection(connectionStringBuilder.ToString());
                    connection.Open();
                    connection.Close();
                });

                txtMessageLog.Text = "Подключение прошло успешно";
                txtMessageLog.Foreground = Brushes.Green;
            }
            catch(Exception ex)
            {
                txtMessageLog.Text = ex.Message;
                txtMessageLog.Foreground = Brushes.Red;
            }
        }

        private async void ButtonClickAccept(object sender, RoutedEventArgs e)
        {
            var connectionStringBuilder = new FbConnectionStringBuilder
            {
                UserID = txtUsername.Text,
                Password = txtPassword.Password,
                Database = txtPath.Text,
                DataSource = txtAddress.Text,
                Port = Int32.Parse(txtPort.Text)
            };

            await DBConnection.SaveConnectionString(connectionStringBuilder.ToString());
            var window = (MainWindow) Window.GetWindow(this);
            window.CheckConnectionStatus();
        }

        private void CmbServerTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedServerType = (ServerType) cmbServerType.SelectedItem;

            if(selectedServerType == ServerType.REMOTE)
            {
                txtAddress.Visibility = Visibility.Visible;
            }
            else
            {
                txtAddress.Visibility = Visibility.Hidden;
                txtAddress.Text = "localhost";
            }
        }

        public void FillFields()
        {
            try
            {
                if (File.Exists(MainWindow.PathToAppsettings))
                {
                    string connectionString = DBConnection.GetConnectionString();
                    var result = new FbConnectionStringBuilder(connectionString);


                    txtUsername.Text = result.UserID;
                    txtPassword.Password = result.Password;
                    txtPath.Text = result.Database;
                    txtAddress.Text = result.DataSource;
                    txtPort.Text = result.Port.ToString();

                    if (txtAddress.Text != "localhost" && txtAddress.Text != "127.0.0.1") 
                        cmbServerType.SelectedItem = ServerType.REMOTE;
                }
            }
            catch(Exception e)
            {

            }
        }
    }
}
