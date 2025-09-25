using System.Collections.ObjectModel;

namespace DbPad.Models
{
    public enum NodeType
    {
        Database,
        Table,
        View,
        StoredProcedure,
        Function,
        Column,
        Schema,
        Other
    }

    public class Node
    {
        public NodeType Type { get; set; } = NodeType.Other;
        public ObservableCollection<Node>? SubNodes { get; }
        public string Title { get; }

        public Node(string title,NodeType type=NodeType.Other)
        {
            Title = title;
        }

        public Node(string title, ObservableCollection<Node> subNodes,NodeType type=NodeType.Other)
        {
            Title = title;
            SubNodes = subNodes;
            Type = type;
        }
    }
}