using DbPad.Adapter.MsSql;
using DbPad.Common.Models;
using DynamicData;
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
        public ObservableCollection<Node> Nodes { get; set; } = new();
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


        public ICommand AddTabCommand { get; }
        public RelayCommand AddConnectionCommand { get; }
        public RelayCommand RemoveTabCommand { get; }

        public ICommand ConnectCommand { get; }


        #region Context menu for tables
        public ICommand Select1000Command { get; set; }
        public ICommand EditDataCommand { get; set; }
        public ICommand DesignTableCommand { get; set; }
        #endregion

        public MainWindowViewModel()
        {
            Tabs = new ObservableCollection<TabItemModel>();

            AddTabCommand = new RelayCommand(AddTab);
            // Эта команда теперь будет обрабатывать параметр, представляющий узел подключения
            AddConnectionCommand = new RelayCommand(async (parameter) => await AddConnectionAsync(parameter));

            Select1000Command = new RelayCommand(Select1000);
            EditDataCommand = new RelayCommand(EditData);
            DesignTableCommand = new RelayCommand(DesignTable);
            RemoveTabCommand = new RelayCommand((parameter) => RemoveTab(parameter as TabItemModel));

            // Инициализация новой команды ConnectCommand
            ConnectCommand = new RelayCommand(async (parameter) => await ConnectAsync(parameter));
            
            // Вызов метода для загрузки подключений при запуске
            LoadConnectionsOnStartup();
        }

        private void RemoveTab(TabItemModel? tab)
        {
            if (tab != null && Tabs.Contains(tab))
            {
                var tabIndex = Tabs.IndexOf(tab);
                Tabs.Remove(tab);

                // If the removed tab was selected, select a new one
                if (SelectedTab == tab)
                {
                    if (Tabs.Any())
                    {
                        SelectedTab = Tabs.Count > tabIndex ? Tabs[tabIndex] : Tabs[tabIndex - 1];
                    }
                    else
                    {
                        // No tabs left, maybe add a new one or set SelectedTab to null
                        SelectedTab = null!; // Or Tabs.Add(new TabItemModel());
                    }
                }
            }
        }

        // Метод для загрузки подключений из файла
        private void LoadConnectionsOnStartup()
        {
            // Путь к файлу connections.json в директории запуска приложения
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


        private void AddTab(object? parameter)
        {
            Tabs.Add(new QueryTabModel());

            Tabs.Add(new ConnectionTabModel());
        }

        // Обновлен для работы с узлами подключения
        private async Task AddConnectionAsync(object? parameter)
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
                    // Очищаем существующие под-узлы и добавляем новые, если узел уже существует
                    // Вместо AddRange, присваиваем новую ObservableCollection.
                    // Свойство SubNodes в классе Node теперь имеет сеттер, который уведомляет UI об изменении.
                    parentConnectionNode.SubNodes = new ObservableCollection<Node>(nodes);
                }
                else
                {
                    //// Если узел родительского подключения не определен, возможно, вы хотите добавить его как новый корневой элемент
                    //// или обновить существующий, если это нежелательное поведение, его можно изменить
                    //Nodes.Clear(); // Рассмотрите, всегда ли это очистка желательна, возможно, вы хотите добавить к определенному родителю.
                    //Nodes.AddRange(nodes);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Строка подключения не предоставлена для AddConnectionAsync.");
            }
        }

        // Новая команда для обработки двойного клика по узлу
        private async Task ConnectAsync(object? parameter)
        {
            if (parameter is Node node && node.Type == NodeType.Connection)
            {
                // Вызываем существующий метод AddConnectionAsync, который загружает базы данных и таблицы
                // для указанного узла подключения.
                await AddConnectionAsync(node);
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
                ConnectionString= selectedNode?.ConnectionString ?? ""
            });
            SelectedTab = Tabs.LastOrDefault();
            (SelectedTab as QueryTabModel).ExecuteSQLCommand.Execute(null);

        }

        private void EditData(object? parameter)
        {

        }

        private void DesignTable(object? parameter)
        {
        }
    }
}
