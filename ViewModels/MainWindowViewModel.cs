using DbPad.Adapter.MsSql;
using DbPad.Common.Models;
using DynamicData;
using Microsoft.Data.SqlClient;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DbPad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Node> Nodes { get; set; } = new();
        public ObservableCollection<TabItemModel> Tabs { get; } = new ObservableCollection<TabItemModel>(new[]
        {
            new TabItemModel { TabCaption="Tab1",Query="Select * from top1",}
        });

        private TabItemModel? _selectedTab;
        public TabItemModel? SelectedTab
        {
            get => _selectedTab;
            set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }


        public ICommand AddTabCommand { get; }
        public RelayCommand AddConnectionCommand { get; }



        #region COMANDS FOR CONTEXT MENU
        public ICommand Select1000Command { get; set; }
        public ICommand EditDataCommand { get; set; }
        public ICommand DesignTableCommand { get; set; }
        #endregion

        public MainWindowViewModel()
        {
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
            var nodes = await MsSqlAdapter.LoadDatabasesAndTablesAsync(MsSqlAdapter.connectionString);

            Nodes.Clear();
            Nodes.AddRange(nodes);
        }

        private void Select1000(object parameter)
        {
            Node? selectedNode = parameter as Node;
            Tabs.Add(new TabItemModel
            {
                TabCaption = $"FROM {selectedNode?.Title}",
                Query = MsSqlAdapter.Select1000Query(selectedNode?.Title ?? ""),
                Results = "-- Results will be displayed here",
                Database = selectedNode?.Database ?? ""
            });
            SelectedTab = Tabs.LastOrDefault();
            SelectedTab?.ExecuteSQLCommand.Execute(null);

        }

        private void EditData(object parameter)
        {

        }

        private void DesignTable(object parameter)
        {
        }
    }
}
