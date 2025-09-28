using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DbPad.ViewModels.Tab
{
    public class DesignTableTabModel : TabItemModel
    {
        private string _tableName = "";
        private string _statusMessage = "Ready";
        private ObservableCollection<ColumnDefinition> _columns = new();
        private ObservableCollection<string> _dataTypes = new() { "INT", "VARCHAR", "FLOAT", "DATETIME", "BIT" };

        public string TableName { get => _tableName; set => SetField(ref _tableName, value); }
        public string StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }
        public ObservableCollection<ColumnDefinition> Columns
        {
            get => _columns;
            set => SetField(ref _columns, value);
        }
        public ObservableCollection<string> DataTypes
        {
            get => _dataTypes;
            set => SetField(ref _dataTypes, value);
        }

        public ICommand AddColumnCommand { get; }
        public ICommand CreateTableCommand { get; }

        public DesignTableTabModel()
        {
            TabCaption = "Design Table Tab";
            AddColumnCommand = new RelayCommand((parameter) => AddColumn());
            CreateTableCommand = new RelayCommand((parameter) => CreateTable());
        }

        private void AddColumn()
        {
            Columns.Add(new ColumnDefinition { Name = "NewColumn", DataType = "VARCHAR", IsNullable = true });
            StatusMessage = "Column added";
        }

        private void CreateTable()
        {
            StatusMessage = $"Table '{TableName}' creation scripted at " + DateTime.Now.ToString();
        }
    }
}