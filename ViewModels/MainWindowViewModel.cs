using DbPad.Models;
using DynamicData;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DbPad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Node> Nodes { get; set; }
        public ObservableCollection<TabItemModel> Tabs { get; }
        public ICommand AddTabCommand { get; }
        public RelayCommand AddConnectionCommand { get; }
        public ICommand Action1Command { get; }
        public ICommand Action2Command { get; }
        public ICommand Action3Command { get; }

        public MainWindowViewModel()
        {
            Nodes = new ObservableCollection<Node>
            {
                new Node("Animals", new ObservableCollection<Node>
                {
                    new Node("Mammals", null)

                })
            };

            Tabs = new ObservableCollection<TabItemModel>();
            AddTabCommand = new RelayCommand(AddTab);
            AddConnectionCommand = new RelayCommand(async () => await AddConnectionAsync());
            Action1Command = new RelayCommand(DoAction1);
            Action2Command = new RelayCommand(DoAction2);
            Action3Command = new RelayCommand(DoAction3);
        }

        private void AddTab()
        {
            Tabs.Add(new TabItemModel());
        }

        private async Task AddConnectionAsync()
        {
            var connectionString = "Server=.\\sqlexpress;Trusted_Connection=True;TrustServerCertificate=True;";
            var nodes = new List<Node>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var databases = new List<Node>();
                using (var cmd = new SqlCommand("SELECT name FROM sys.databases", connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dbName = reader.GetString(0);
                        databases.Add(new Node(dbName, new ObservableCollection<Node>(), NodeType.Database));
                    }
                }

                foreach (var db in databases)
                {
                    using (var cmd = new SqlCommand($"USE [{db.Title}]; SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            db.SubNodes.Add(new Node(reader.GetString(0), NodeType.Table));
                        }
                    }
                }

                nodes.AddRange(databases);
            }
            Nodes.Clear();
            Nodes.AddRange(nodes);
        }

        private void DoAction1()
        {
            // Ваш код для действия 1
        }

        private void DoAction2()
        {
            // Ваш код для действия 2
        }

        private void DoAction3()
        {
            // Ваш код для действия 3
        }
    }
}
