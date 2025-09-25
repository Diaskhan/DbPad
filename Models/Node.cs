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

        public string Glyph
        {
            get
            {
                return Type switch
                {
                    NodeType.Database         => "avares://DbPad/Assets/Icons/database.svg",
                    NodeType.Table            => "avares://DbPad/Assets/Icons/table.svg",
                    NodeType.View             => "avares://DbPad/Assets/Icons/view.svg",
                    NodeType.StoredProcedure  => "avares://DbPad/Assets/Icons/storedprocedure.svg",
                    NodeType.Function         => "avares://DbPad/Assets/Icons/function.svg",
                    NodeType.Column           => "avares://DbPad/Assets/Icons/column.svg",
                    NodeType.Schema           => "avares://DbPad/Assets/Icons/schema.svg",
                    _                        => "avares://DbPad/Assets/Icons/other.svg"
                };
            }
        }
    }
}