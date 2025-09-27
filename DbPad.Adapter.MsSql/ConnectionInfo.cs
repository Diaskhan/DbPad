using System.Text.Json.Serialization;

namespace DbPad.Common.Models
{

    public class MsSqlConnectionInfo
    {
        [JsonPropertyName("server")]
        public string Server { get; set; } = string.Empty;
        [JsonPropertyName("database")]
        public string Database { get; set; } = string.Empty;
        [JsonPropertyName("authenticationType")]
        public string AuthenticationType { get; set; } = string.Empty;
        [JsonPropertyName("user")]
        public string User { get; set; } = string.Empty;
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
        [JsonPropertyName("savePassword")]
        public bool SavePassword { get; set; }
        [JsonPropertyName("connectionString")]
        public string ConnectionString { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonIgnore]
        public string DisplayName => string.IsNullOrWhiteSpace(Database) ? Server : $"{Server} [{Database}]";
    }

    public class ConnectionFileRoot
    {
        [JsonPropertyName("mssql.connections")]
        public List<MsSqlConnectionInfo> MssqlConnections { get; set; } = new List<MsSqlConnectionInfo>();
    }
}