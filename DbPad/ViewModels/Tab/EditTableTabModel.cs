using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows.Input;

namespace DbPad.ViewModels.Tab
{
    public class EditTableTabModel : TabItemModel
    {
        private string _tableName = "";
        private string _statusMessage = "Ready";
        private ObservableCollection<ExpandoObject> _tableData = new();

        public string TableName { get => _tableName; set => SetField(ref _tableName, value); }
        public string StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }
        public ObservableCollection<ExpandoObject> TableData
        {
            get => _tableData;
            set => SetField(ref _tableData, value);
        }

        public ICommand SaveTableChangesCommand { get; }
        public ICommand RefreshTableCommand { get; }

        public EditTableTabModel()
        {
            TabCaption = "Edit Table Tab";
            SaveTableChangesCommand = new RelayCommand((parameter) => SaveTableChanges());
            RefreshTableCommand = new RelayCommand((parameter) => RefreshTable());
        }

        private void SaveTableChanges()
        {
            StatusMessage = "Changes saved at " + DateTime.Now.ToString();
        }

        private void RefreshTable()
        {
            StatusMessage = "Table refreshed at " + DateTime.Now.ToString();
            // Placeholder for actual table refresh logic
            TableData = new ObservableCollection<ExpandoObject>();
        }
    }
}