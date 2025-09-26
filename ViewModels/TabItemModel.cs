using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DbPad.ViewModels
{
    public class TabItemModel : INotifyPropertyChanged
    {
        public string connectionString = "Server=.\\sqlexpress;Trusted_Connection=True;TrustServerCertificate=True;";
        private string _tabCaption = "Tab1";
        private string _query = "Text1";
        private string _results = "Text2";
        private string _database = "";

        public string TabCaption
        {
            get => _tabCaption;
            set => SetField(ref _tabCaption, value);
        }

        public string Query
        {
            get => _query;
            set => SetField(ref _query, value);
        }

        public string Results
        {
            get => _results;
            set => SetField(ref _results, value);
        }
        public string Database
        {
            get => _database;
            set => SetField(ref _database, value);
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

        private async Task ExecuteSQLAsync(object? parameter)
        {
            if ( string.IsNullOrWhiteSpace(Query))
            {
                return;
            }


            try
            {

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    connection.ChangeDatabase(Database);

                    using (var cmd = new SqlCommand(Query, connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var results = new System.Text.StringBuilder();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            results.Append(reader.GetName(i)).Append("\t");
                        }
                        results.AppendLine();

                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                results.Append(reader[i]?.ToString()).Append("\t");
                            }
                            results.AppendLine();
                        }

                        Results = results.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Results = "Error executing: " + ex.Message;
            }
        }
    }
}
