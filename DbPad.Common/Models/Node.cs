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

        public Node(string title, string database, NodeType type)
        {
            Title = title;
            Database = database;
            Type = type;
        }

        public Node(string title, string database, ObservableCollection<Node> subNodes, NodeType type = NodeType.Other)
        {
            Title = title;
            Database = database;
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
            return Type switch
            {
                NodeType.Connection => "avares://DbPad/Assets/Icons/connection.svg", // Пример пути
                NodeType.Database => "avares://DbPad/Assets/Icons/database.svg",
                NodeType.Schema => "avares://DbPad/Assets/Icons/schema.svg",
                NodeType.Table => "avares://DbPad/Assets/Icons/table.svg",
                NodeType.View => "avares://DbPad/Assets/Icons/view.svg",
                NodeType.StoredProcedure => "avares://DbPad/Assets/Icons/storedprocedure.svg",
                NodeType.Function => "avares://DbPad/Assets/Icons/function.svg",
                NodeType.Column => "avares://DbPad/Assets/Icons/column.svg",
                _ => "avares://DbPad/Assets/Icons/other.svg" // Иконка по умолчанию
            };
        }
    }
}