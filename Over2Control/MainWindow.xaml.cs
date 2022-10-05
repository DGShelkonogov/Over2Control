using Newtonsoft.Json.Linq;
using Over2Control.Services;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Over2Control
{
    public partial class MainWindow : Window
    {
        public static string PathToAppsettings = "appsettings.json";

        public MainWindow()
        {
            InitializeComponent();
            CheckConnectionStatus();

            if (!File.Exists(PathToAppsettings))
            {
                string output = "{ \"WorkerOptions\": \n" +
                    "{ \"SkudDbConnectionString\": \"\", \n" +
                    "\"RetainedFileCountLimit\": 30, \n" +
                    "\"LogFolerPath\": \"C:\\\\ProgramData\\\\Over2\\\\\", \n" +
                    "\"ServeIp\": \"172.19.0.95\", \n" +
                    "\"MqttServer\": \"194.87.237.67\", \n" +
                    "\"appName\": \"Over_APB\", \n" +
                    "\"ServiceName\": \"Over2_APB\", \n" +
                    "\"Controllers\": {}, \n" +
                    "\"Logging\": { \n" +
                    "\"LogLevel\": { \n" +
                    "\"Default\": \"Information\", \n" +
                    "\"Microsoft.Hosting.Lifetime\": \"Information\" \n} \n} \n} \n}";

                FileInfo fi = new FileInfo(PathToAppsettings);
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(output);
                }
            }
        }

        public async void CheckConnectionStatus()
        {
            var status = await DBConnection.CheckConnection();

            switch (status)
            {
                case DBConnectionStatus.OPEN:
                    {
                        ellipseDatabaseStatus.Fill = Brushes.Green;
                        txtDatabaseStatus.Text = "Подключение к базе данных настроено";
                        break;
                    }
                case DBConnectionStatus.MISSING:
                    {
                        ellipseDatabaseStatus.Fill = Brushes.Red;
                        txtDatabaseStatus.Text = "Подключение к базе данных отсутствует";
                        break;
                    }

                case DBConnectionStatus.NOT_CONFIGURATED:
                    {
                        ellipseDatabaseStatus.Fill = Brushes.Orange;
                        txtDatabaseStatus.Text = "Подключение к базе данных не настроено";
                        break;
                    }
            }
        }

        
    }
}
