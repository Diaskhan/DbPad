using System.Collections.ObjectModel;

namespace DbPad.Models
{
    public class Node
    {
        public NodeType Type { get; } = NodeType.Other;
        public ObservableCollection<Node>? SubNodes { get; }
        public string Title { get; }
        public string Database { get; }

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
            SubNodes = subNodes;
            Type = type;
        }
    }
}