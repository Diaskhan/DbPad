namespace DbPad.Models
{
    public enum NodeType
    {
        Connection,
        Database,
        Table,
        View,
        StoredProcedure,
        Function,
        Column,
        Schema,
        Other,
    }


    //public string Glyph
    //{
    //    get
    //    {
    //        return Type switch
    //        {
    //            NodeType.Database         => "avares://DbPad/Assets/Icons/database.svg",
    //            NodeType.Table            => "avares://DbPad/Assets/Icons/table.svg",
    //            NodeType.View             => "avares://DbPad/Assets/Icons/view.svg",
    //            NodeType.StoredProcedure  => "avares://DbPad/Assets/Icons/storedprocedure.svg",
    //            NodeType.Function         => "avares://DbPad/Assets/Icons/function.svg",
    //            NodeType.Column           => "avares://DbPad/Assets/Icons/column.svg",
    //            NodeType.Schema           => "avares://DbPad/Assets/Icons/schema.svg",
    //            _                        => "avares://DbPad/Assets/Icons/other.svg"
    //        };
    //    }
    //}
}
