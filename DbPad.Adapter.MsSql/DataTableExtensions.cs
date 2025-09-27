using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;


namespace DbPad.Adapter.MsSql
{

    public static class DataTableExtensions
    {
        public static ObservableCollection<ExpandoObject> ToExpandoCollection(this DataTable dt)
        {
            var collection = new ObservableCollection<ExpandoObject>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic expando = new ExpandoObject();
                var dict = (IDictionary<string, object>)expando;
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                collection.Add(expando);
            }
            return collection;
        }
    }
}
