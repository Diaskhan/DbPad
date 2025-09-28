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
    public class TabItemModel : INotifyPropertyChanged
    {
        private string _tabCaption = "Tab1";
        private string _query = "Text1";
        private string _results = "Text2";
        private string _database = "";
        private string _connectionString = "";

        public string TabCaption { get => _tabCaption; set => SetField(ref _tabCaption, value); }
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

        public TabItemModel()
        {
            ExecuteSQLCommand = new RelayCommand(async (parameter) => await ExecuteSQLAsync(parameter));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand ExecuteSQLCommand { get; }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public async Task ExecuteSQLAsync(object? parameter)
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
}
