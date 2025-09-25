using System.Collections.ObjectModel;

namespace DbPad.Models
{
    public class Node
    {
        public NodeType Type { get; set; } = NodeType.Other;
        public ObservableCollection<Node>? SubNodes { get; }
        public string Title { get; }

        public Node(string title, NodeType type)
        {
            Title = title;
            Type = type;
        }

        public Node(string title, ObservableCollection<Node> subNodes, NodeType type = NodeType.Other)
        {
            Title = title;
            SubNodes = subNodes;
            Type = type;
        }
    }
}