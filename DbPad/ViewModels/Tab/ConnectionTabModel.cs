using DbPad.Common.Models;
using Microsoft.Data.SqlClient; // Ensure this is the correct namespace, as it often is for the newer lib
using System;
using System.Windows.Input;

namespace DbPad.ViewModels.Tab
{
    public class ConnectionTabModel : TabItemModel
    {
        private string _server = "";
        private string _database = "";
        private string _username = "";
        private string _password = "";
        private string _connectionStatus = "Not connected";

        public string Server { get => _server; set => SetField(ref _server, value); }
        public string Database { get => _database; set => SetField(ref _database, value); }
        public string Username { get => _username; set => SetField(ref _username, value); }
        public string Password { get => _password; set => SetField(ref _password, value); }
        public string ConnectionStatus { get => _connectionStatus; set => SetField(ref _connectionStatus, value); }

        public ICommand TestConnectionCommand { get; }
        public ICommand SaveConnectionCommand { get; }
        // Event to notify the ViewModel that a connection has been saved
        public event Action<Node>? ConnectionSaved;

        public ConnectionTabModel()
        {
            TabCaption = "New connection";
            TestConnectionCommand = new RelayCommand((parameter) => TestConnection());
            SaveConnectionCommand = new RelayCommand((parameter) => SaveConnection()); // Add CanSaveConnection logic as needed
        }

        private void TestConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = Server;
            builder.InitialCatalog = Database;

            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                builder.UserID = Username;
                builder.Password = Password;
            }
            else
            {
                // Use integrated security if username/password are empty
                builder.IntegratedSecurity = true;
                builder.TrustServerCertificate = true; // Often needed for trusted connections
            }

            string connectionString = builder.ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    ConnectionStatus = $"Connected to {Database} on {Server} at {DateTime.Now.ToString()}";
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Connection failed: {ex.Message} at {DateTime.Now.ToString()}";
            }
        }

        private void SaveConnection()
        {
            //ArgumentException.ThrowIfNullOrWhiteSpace(Server);
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = Server;
            builder.InitialCatalog = Database;

            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                builder.UserID = Username;
                builder.Password = Password;
            }
            else
            {
                // Use integrated security if username/password are empty
                builder.IntegratedSecurity = true;
                builder.TrustServerCertificate = true; // Often needed for trusted connections
            }

            string connectionString = builder.ConnectionString;
            var newConnectionNode = new Node(Server, Database, NodeType.Connection, connectionString);
            ConnectionSaved?.Invoke(newConnectionNode);

            Console.WriteLine($"Connection saved: {newConnectionNode.Title}");
            // In a real application, you would add this node to your connection tree management.
        }
    }
}