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


        #region COMANDS FOR CONTEXT MENU
        public ICommand Select1000Command { get; set; }
        public ICommand EditDataCommand { get; set; }
        public ICommand DesignTableCommand { get; set; }
        #endregion

        public MainWindowViewModel()
        {
            Nodes = new ObservableCollection<Node>
            {
                new Node("Empty Connection", new ObservableCollection<Node>(),NodeType.Connection)
            };

            Tabs = new ObservableCollection<TabItemModel>();

            AddTabCommand = new RelayCommand(AddTab);
            AddConnectionCommand = new RelayCommand(async (parameter) => await AddConnectionAsync(null));

            Select1000Command = new RelayCommand(Select1000);
            EditDataCommand = new RelayCommand(EditData);
            DesignTableCommand = new RelayCommand(DesignTable);
        }

        private void AddTab(object? parameter)
        {
            Tabs.Add(new TabItemModel());
        }

        private async Task AddConnectionAsync(object? parameter)
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

        private void Select1000(object parameter)
        {
            Node? selectedNode = parameter as Node;
            Tabs.Add(new TabItemModel
            {
                TabCaption= $"FROM {selectedNode.Title}",
                Text1 = $"SELECT TOP 1000 * FROM [{selectedNode.Title}];",
                Text2 = "-- Results will be displayed here"
            });

        }

        private void EditData(object parameter)
        {
         
        }

        private void DesignTable(object parameter)
        {
        }
    }
}
