using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices; // Для CallerMemberName

namespace DbPad.Common.Models
{
    public class Node : INotifyPropertyChanged // Node должен реализовать INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Поле для Node.SubNodes
        private ObservableCollection<Node>? _subNodes;

        public string Title { get; }
        public string Database { get; }
        public string ConnectionString { get; }
        public NodeType Type { get; }

        public string IconSource => GetIconSource();

        public ObservableCollection<Node>? SubNodes
        {
            get => _subNodes;
            set
            {
                // Убедитесь, что уведомление об изменении свойства происходит при замене коллекции
                if (_subNodes != value)
                {
                    _subNodes = value;
                    OnPropertyChanged();
                }
            }
        }

        public Node(string title, string database, NodeType type, string connectionString)
        {
            Title = title;
            Database = database;
            Type = type;
            ConnectionString = connectionString;
        }

        public Node(string title, string database, ObservableCollection<Node> subNodes, string connectionString, NodeType type = NodeType.Other)
        {
            Title = title;
            Database = database;
            ConnectionString = connectionString;
            SubNodes = subNodes; // Используем свойство, чтобы вызвать OnPropertyChanged при инициализации
            Type = type;
        }

        // Метод для вызова события PropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Метод для определения пути к иконке
        private string GetIconSource()
        {
            // Предполагается, что у вас есть SVG-файлы в папке Assets/Icons
            if (Type != NodeType.Connection)
            {
                return Type switch
                {
                    NodeType.Database => "Assets/database.png",
                    NodeType.Table => "Assets/table.png"
                };
            }

            return Type switch
            {
                NodeType.Connection => "Assets/sql.connection.png", // Пример пути
            };
        }
    }
}