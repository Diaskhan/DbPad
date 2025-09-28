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

// Add these necessary classes (if not already present in a shared Models or Data directory)
// These might be defined in a separate project or within the same project but not shown in the context.
// For demonstration, I'm including them here.

public class ConnectionFileRoot // Example structure for JSON
{
    public List<MssqlConnectionInfo>? MssqlConnections { get; set; } = [];
    // Add other connection types if needed
}

public class MssqlConnectionInfo
{
    public string DisplayName { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}


namespace DbPad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string ConnectionsFilePath = "connections.json"; // Define the file path

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
                    SaveConnectionsToFile(); // Save after removing
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
            string filePath = Path.Combine(AppContext.BaseDirectory, ConnectionsFilePath);

            if (File.Exists(filePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(filePath);
                    var rootList = JsonSerializer.Deserialize<List<ConnectionFileRoot>>(jsonString);

                    if (rootList != null)
                    {
                        foreach (var rootItem in rootList) // Iterate through potential root items if roots list is not always just one.
                        {
                            if (rootItem?.MssqlConnections != null)
                            {
                                foreach (var info in rootItem.MssqlConnections)
                                {
                                    Nodes.Add(new Node(info.DisplayName, info.Database, NodeType.Connection, info.ConnectionString));
                                }
                            }
                        }
                    }
                }
                catch (JsonException jsonEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deserializing connections file: {jsonEx.Message}");
                    // Optionally, handle corrupted file: backup or clear
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading connections: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"File not found: {filePath}");
            }
        }

        private void SaveConnectionsToFile()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, ConnectionsFilePath);

            var connectionsToSave = new List<ConnectionFileRoot>();
            var mssqlConnections = Nodes
                .Where(n => n.Type == NodeType.Connection)
                .Select(n => new MssqlConnectionInfo
                {
                    DisplayName = n.Title,
                    Database = n.Database,
                    ConnectionString = n.ConnectionString
                })
                .ToList();

            if (mssqlConnections.Any())
            {
                connectionsToSave.Add(new ConnectionFileRoot { MssqlConnections = mssqlConnections });
            }

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(connectionsToSave, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving connections: {ex.Message}");
            }
        }

        private void NewQuery(object? parameter)
        {
            Tabs.Add(new QueryTabModel());
            SelectedTab = Tabs.LastOrDefault();
        }

        private void AddConnection(object? parameter)
        {
            var connectionTab = new ConnectionTabModel();
            connectionTab.ConnectionSaved += OnConnectionSaved;
            Tabs.Add(connectionTab);
            SelectedTab = Tabs.LastOrDefault();
        }

        private void OnConnectionSaved(Node connectionNode)
        {
            if (!Nodes.Any(n => n.ConnectionString == connectionNode.ConnectionString && n.Title == connectionNode.Title))
            {
                Nodes.Add(connectionNode);
                SaveConnectionsToFile(); // Save after adding to Nodes collection
            }

            // Close the connection tab
            var tabToClose = Tabs.OfType<ConnectionTabModel>().FirstOrDefault(t => t.Database == connectionNode.Database);
            if (tabToClose != null)
            {
                RemoveTab(tabToClose);
            }
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
                // Ensure MsSqlAdapter exists and is referenced
                var nodes = await MsSqlAdapter.LoadDatabasesAndTablesAsync(connectionString);

                if (parentConnectionNode != null)
                {
                    parentConnectionNode.SubNodes = new ObservableCollection<Node>(nodes);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Connection string not provided for AddConnectionAsync.");
            }
        }

        private async Task ConnectAsync(object? parameter)
        {
            if (parameter is Node node && node.Type == NodeType.Connection)
            {
                await ConnectToDbAsync(node);
                System.Diagnostics.Debug.WriteLine($"Connecting to database: {node.Title}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Invalid node type for connection.");
            }
        }

        private void Select1000(object? parameter)
        {
            Node? selectedNode = parameter as Node;
            if (selectedNode != null)
            {
                Tabs.Add(new QueryTabModel
                {
                    TabCaption = $"FROM {selectedNode.Title}",
                    Query = MsSqlAdapter.Select1000Query(selectedNode.Title), // Assuming Select1000Query expects title, adjust if it needs DB name or conn string
                    Results = "-- Results will be here",
                    Database = selectedNode.Database,
                    ConnectionString = selectedNode.ConnectionString
                });
                SelectedTab = Tabs.LastOrDefault();
                // Execute the command, ensuring it's cast correctly
                if (SelectedTab is QueryTabModel queryTab)
                {
                    queryTab.ExecuteSQLCommand.Execute(null);
                }
            }
        }

        private void EditData(object? parameter) { }

        private void DesignTable(object? parameter) { }
    }
}