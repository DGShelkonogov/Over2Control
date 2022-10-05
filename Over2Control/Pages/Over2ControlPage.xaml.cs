using Newtonsoft.Json;
using Over2Control.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Over2Control.Pages
{
    public partial class Over2ControlPage : Page
    {

        private ServiceControllerInfo _service;
        public Over2ControlPage()
        {
            try
            {
                InitializeComponent();

                var pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);


                if (hasAdministrativeRight)
                {
                    InitService();
                    FillFuilds();
                }
                else
                {
                    txtServiceName.Text = "Не определено";
                    txtServiceStatusName.Text = "Не определено";
                    txtServiceTypeName.Text = "Не определено";
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = false;
                    btnFind.IsEnabled = false;
                    txtInfo.Text = "Чтобы работать со службой, необходимо запустить программу от имени администратора";
                }

               
            }
            catch(Exception e)
            {
                MessageBox.Show($"Ошибка произошла на странице 'Служба Over 2'. Ошибка: {e.Message}");
            }
        }

        public void InitService()
        {
            string json = File.ReadAllText(MainWindow.PathToAppsettings);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            string ServiceName = jsonObj["WorkerOptions"]["ServiceName"];


            var scServices = ServiceController.GetServices();
            var serviceController = scServices.FirstOrDefault(x => x.ServiceName.ToLower().Equals(ServiceName.ToLower()));

            if (serviceController != null)
            {
                _service = new ServiceControllerInfo(serviceController);
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = true;
                btnFind.Visibility = Visibility.Hidden;
            }
            else
            {
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = false;
                btnFind.Visibility = Visibility.Visible;
            }
        }

        public void FillFuilds()
        {
            if (_service != null)
            {
                txtServiceName.Text = _service.ServiceName;
                txtServiceStatusName.Text = _service.ServiceStatusName;
                txtServiceTypeName.Text = _service.ServiceTypeName;
            }
            else
            {
                txtServiceName.Text = "Не определено";
                txtServiceStatusName.Text = "Не определено";
                txtServiceTypeName.Text = "Не определено";
            }
        }

        private async void ButtonClickStart(object sender, RoutedEventArgs e)
        {
            if (_service.EnableStart)
            {
                await Task.Run(() => {
                    _service.Controller.Start();
                    _service.Controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                });  
            }
            else
                MessageBox.Show("Службу невозможно запустить");

            FillFuilds();
        }

        private async void ButtonClickStop(object sender, RoutedEventArgs e)
        {
            if (_service.EnableStop)
            {
                await Task.Run(() => {
                    _service.Controller.Stop();
                    _service.Controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                });
            }
            else
                MessageBox.Show("Службу невозможно остановить");

            FillFuilds();
        }

        private async void ButtonClickFindService(object sender, RoutedEventArgs e)
        {
            InitService();
        }

        private void ButtonOpenLogFile(object sender, RoutedEventArgs e)
        {
            try
            {
                string json = File.ReadAllText(MainWindow.PathToAppsettings);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);

                string LogFolerPath = jsonObj["WorkerOptions"]["LogFolerPath"];
                System.Diagnostics.Process.Start("explorer", LogFolerPath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
