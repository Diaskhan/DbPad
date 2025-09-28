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

        public ConnectionTabModel()
        {
            TabCaption = "New connection";
            TestConnectionCommand = new RelayCommand((parameter) => TestConnection());
        }

        private void TestConnection()
        {
            ConnectionStatus = "Connection tested at " + DateTime.Now.ToString();
            // Placeholder for actual connection test logic
        }
    }
}