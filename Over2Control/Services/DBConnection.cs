using FirebirdSql.Data.FirebirdClient;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Over2Control.Services
{
    public enum DBConnectionStatus
    {
        OPEN,
        MISSING,
        NOT_CONFIGURATED
    }

    public static class DBConnection
    {
        public async static Task<FbConnection> GetFbConnection()
        {
            if (File.Exists(MainWindow.PathToAppsettings))
            {
                try
                {
                    string connectionString = GetConnectionString();
                    var connection = new FbConnection(connectionString);
                    connection.Open();
                    connection.Close();
                    return connection;
                }
                catch (Exception e)
                {
                    
                }
            }
            return null;
        }

        public static async Task SaveConnectionString(string connectionStringJson)
        {
            connectionStringJson = connectionStringJson.Replace("user id", "User");
            connectionStringJson = connectionStringJson.Replace("password", "Password");
            connectionStringJson = connectionStringJson.Replace("initial catalog", "Database");
            connectionStringJson = connectionStringJson.Replace("data source", "DataSource");
            connectionStringJson = connectionStringJson.Replace("port number", "Port");
            connectionStringJson += ";Dialect = 3; Charset = win1251; Role =; Connection lifetime = 15; " +
                "Pooling = true; MinPoolSize = 0; MaxPoolSize = 50; Packet Size = 8192; ServerType = 0;";
            await Task.Run(() => {
                string json = File.ReadAllText(MainWindow.PathToAppsettings);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                jsonObj["WorkerOptions"]["SkudDbConnectionString"] = connectionStringJson;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(MainWindow.PathToAppsettings, output);
            });   
        }

        public async static Task<DBConnectionStatus> CheckConnection()
        {
            if (!File.Exists(MainWindow.PathToAppsettings))
                return DBConnectionStatus.NOT_CONFIGURATED;

            string connectionString = GetConnectionString();

             if (connectionString.ToString().Length == 0)
                return DBConnectionStatus.NOT_CONFIGURATED;

            try
            {
                await Task.Run(() => {
                    var connection = new FbConnection(connectionString);
                    connection.Open();
                    connection.Close();
                });
            }
            catch (Exception e)
            {
                return DBConnectionStatus.MISSING;
            }

            return DBConnectionStatus.OPEN;
        }

        public  static string GetConnectionString()
        {
            string json = File.ReadAllText(MainWindow.PathToAppsettings);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            string result = jsonObj["WorkerOptions"]["SkudDbConnectionString"];
            string newResult = result.Replace("\\", "\\\\");
            return newResult;
        }
    }
}
