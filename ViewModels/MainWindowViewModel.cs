using DbPad.Models;
using DynamicData;
using Microsoft.Data.SqlClient;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DbPad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Node> Nodes { get; set; }
        public ObservableCollection<TabItemModel> Tabs { get; } = new();

        private TabItemModel? _selectedTab;
        public TabItemModel? SelectedTab
        {
            get => _selectedTab;
            set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }


        public ICommand AddTabCommand { get; }
        public RelayCommand AddConnectionCommand { get; }

        public ICommand ExecuteSQLCommand { get; }



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
            ExecuteSQLCommand = new RelayCommand(async (parameter) => await ExecuteSQLAsync(parameter));


            Select1000Command = new RelayCommand(Select1000);
            EditDataCommand = new RelayCommand(EditData);
            DesignTableCommand = new RelayCommand(DesignTable);
        }

        private async Task ExecuteSQLAsync(object? parameter)
        {
            if (_selectedTab == null || string.IsNullOrWhiteSpace(_selectedTab.Text1))
            {
                return;
            }


            try
            {
                var connectionString = "Server=.\\sqlexpress;Trusted_Connection=True;TrustServerCertificate=True;";

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    //connection.ChangeDatabase("master");

                    using (var cmd = new SqlCommand(_selectedTab.Text1, connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var results = new System.Text.StringBuilder();

                        // Заголовки колонок
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            results.Append(reader.GetName(i)).Append("\t");
                        }
                        results.AppendLine();

                        // Данные
                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                results.Append(reader[i]?.ToString()).Append("\t");
                            }
                            results.AppendLine();
                        }

                        _selectedTab.Text2 = results.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _selectedTab.Text2 = "Ошибка выполнения: " + ex.Message;
            }
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
                            db.SubNodes?.Add(new Node(reader.GetString(0), NodeType.Table));
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
                TabCaption = $"FROM {selectedNode?.Title}",
                Text1 = $"SELECT TOP 1000 * FROM [{selectedNode?.Title}];",
                Text2 = "-- Results will be displayed here"
            });
            SelectedTab = Tabs.LastOrDefault();

        }

        private void EditData(object parameter)
        {

        }

        private void DesignTable(object parameter)
        {
        }
    }
}
