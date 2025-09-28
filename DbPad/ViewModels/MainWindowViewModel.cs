using DbPad.Adapter.MsSql;
using DbPad.Common.Models;
using DbPad.ViewModels.Tab;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DbPad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Node> Nodes { get; set; } = [];
        public Node? SelectedNode { get; set; }
        public ObservableCollection<TabItemModel> Tabs { get; } = new ObservableCollection<TabItemModel>(new[]
        {
            new QueryTabModel { TabCaption="Tab1",Query="Select * from top1",}
        });

        private TabItemModel? _selectedTab;
        public TabItemModel? SelectedTab
        {
            get => _selectedTab;
            set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }


        public ICommand NewQueryCommand { get; }
        public RelayCommand AddConnectionCommand { get; }
        public RelayCommand RemoveConnectionCommand { get; }
        public RelayCommand RemoveTabCommand { get; }

        public ICommand ConnectCommand { get; }


        #region Context menu for tables
        public ICommand Select1000Command { get; set; }
        public ICommand EditDataCommand { get; set; }
        public ICommand DesignTableCommand { get; set; }
        #endregion

        public MainWindowViewModel()
        {
            Tabs = [];

            NewQueryCommand = new RelayCommand(NewQuery);
            AddConnectionCommand = new RelayCommand(AddConnection);
            RemoveConnectionCommand = new RelayCommand(RemoveConnection);

            Select1000Command = new RelayCommand(Select1000);
            EditDataCommand = new RelayCommand(EditData);
            DesignTableCommand = new RelayCommand(DesignTable);
            RemoveTabCommand = new RelayCommand((parameter) => RemoveTab(parameter as TabItemModel));
            ConnectCommand = new RelayCommand(async (parameter) => await ConnectAsync(parameter));

            LoadConnectionsOnStartup();
        }
        private void RemoveConnection(object? parameter)
        {
            if (SelectedNode?.Type == NodeType.Connection)
            {
                if (Nodes.Contains(SelectedNode))
                {
                    Nodes.Remove(SelectedNode);
                }
            }
        }

        private void RemoveTab(TabItemModel? tab)
        {
            if (tab != null && Tabs.Contains(tab))
            {
                var tabIndex = Tabs.IndexOf(tab);
                Tabs.Remove(tab);

                if (SelectedTab == tab)
                {
                    if (Tabs.Any())
                    {
                        SelectedTab = Tabs.Count > tabIndex ? Tabs[tabIndex] : Tabs[tabIndex - 1];
                    }
                    else
                    {
                        SelectedTab = null!;
                    }
                }
            }
        }

        private void LoadConnectionsOnStartup()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "connections.json");

            if (File.Exists(filePath))
            {

                string jsonString = File.ReadAllText(filePath);
                var rootList = JsonSerializer.Deserialize<List<ConnectionFileRoot>>(jsonString);

                if (rootList != null && rootList.Any())
                {
                    var firstRoot = rootList.First();
                    if (firstRoot?.MssqlConnections != null)
                    {
                        foreach (var info in firstRoot.MssqlConnections)
                        {

                            Nodes.Add(new Node(info.DisplayName, info.Database, NodeType.Connection, info.ConnectionString));
                        }
                    }
                }

            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Файл подключений не найден по адресу: {filePath}");
            }
        }

        private void NewQuery(object? parameter)
        {
            Tabs.Add(new QueryTabModel());
            SelectedTab = Tabs.LastOrDefault();
        }

        private void AddConnection(object? parameter)
        {
            Tabs.Add(new ConnectionTabModel());
            SelectedTab = Tabs.LastOrDefault();
        }

        private async Task ConnectToDbAsync(object? parameter)
        {
            string? connectionString = null;
            Node? parentConnectionNode = null;

            if (parameter is Node node && node.Type == NodeType.Connection)
            {
                connectionString = node.ConnectionString;
                parentConnectionNode = node;
            }
            else if (parameter is string paramString && !string.IsNullOrEmpty(paramString))
            {
                connectionString = paramString;
            }

            if (!string.IsNullOrEmpty(connectionString))
            {
                var nodes = await MsSqlAdapter.LoadDatabasesAndTablesAsync(connectionString);

                if (parentConnectionNode != null)
                {
                    parentConnectionNode.SubNodes = new ObservableCollection<Node>(nodes);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Строка подключения не предоставлена для AddConnectionAsync.");
            }
        }

        private async Task ConnectAsync(object? parameter)
        {
            if (parameter is Node node && node.Type == NodeType.Connection)
            {
                await ConnectToDbAsync(node);
                System.Diagnostics.Debug.WriteLine($"Подключение к базе данных: {node.Title}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Неверный тип узла для подключения.");
            }
        }

        private void Select1000(object? parameter)
        {
            Node? selectedNode = parameter as Node;
            Tabs.Add(new QueryTabModel
            {
                TabCaption = $"FROM {selectedNode?.Title}",
                Query = MsSqlAdapter.Select1000Query(selectedNode?.Title ?? ""),
                Results = "-- Результаты будут здесь",
                Database = selectedNode?.Database ?? "",
                ConnectionString = selectedNode?.ConnectionString ?? ""
            });
            SelectedTab = Tabs.LastOrDefault();
            (SelectedTab as QueryTabModel)?.ExecuteSQLCommand.Execute(null);

        }

        private void EditData(object? parameter)
        {

        }

        private void DesignTable(object? parameter)
        {
        }
    }
}
