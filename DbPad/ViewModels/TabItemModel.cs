using DbPad.Adapter.MsSql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DbPad.ViewModels
{
    public abstract class TabItemModel : INotifyPropertyChanged
    {
        private string _tabCaption = "Tab";
        public string TabCaption { get => _tabCaption; set => SetField(ref _tabCaption, value); }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }

    public class QueryTabModel : TabItemModel
    {
        private string _query = "Text1";
        private string _results = "Text2";
        private string _database = "";
        private string _connectionString = "";

        public string Query { get => _query; set => SetField(ref _query, value); }
        public string Results { get => _results; set => SetField(ref _results, value); }
        public string Database { get => _database; set => SetField(ref _database, value); }
        public string ConnectionString { get => _connectionString; set => SetField(ref _connectionString, value); }

        private ObservableCollection<ExpandoObject> _dbResults = new();
        public ObservableCollection<ExpandoObject> DbResults
        {
            get => _dbResults;
            set => SetField(ref _dbResults, value);
        }

        public ICommand ExecuteSQLCommand { get; }

        public QueryTabModel()
        {
            TabCaption = "Query Tab";
            ExecuteSQLCommand = new RelayCommand(async (parameter) => await ExecuteSQLAsync(parameter));
        }

        private async Task ExecuteSQLAsync(object? parameter)
        {
            try
            {
                var result = await MsSqlAdapter.ExecuteSQLAsync(_query, _database, _connectionString);
                if (result != null)
                {
                    DbResults = result.ToExpandoCollection();
                    Results = $"Returned {result.Rows.Count} rows";
                }
                else
                {
                    Results = "Error executing SQL";
                    DbResults = new();
                }
            }
            catch (Exception ex)
            {
                Results = $"Error executing SQL: {ex.Message}";
                DbResults = new();
            }
        }
    }

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
            RefreshTableCommand = new RelayCommand(async (parameter) => await RefreshTableAsync());
        }

        private void SaveTableChanges()
        {
            StatusMessage = "Changes saved at " + DateTime.Now.ToString();
        }

        private async Task RefreshTableAsync()
        {
            StatusMessage = "Table refreshed at " + DateTime.Now.ToString();
            // Placeholder for actual table refresh logic
            TableData = new ObservableCollection<ExpandoObject>();
        }
    }

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

    public class ColumnDefinition
    {
        public string Name { get; set; } = "";
        public string DataType { get; set; } = "";
        public bool IsNullable { get; set; }
    }

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
            TabCaption = "Connection Tab";
            TestConnectionCommand = new RelayCommand((parameter) => TestConnection());
        }

        private void TestConnection()
        {
            ConnectionStatus = "Connection tested at " + DateTime.Now.ToString();
            // Placeholder for actual connection test logic
        }
    }
}