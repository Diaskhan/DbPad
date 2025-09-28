using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DbPad.ViewModels.Tab
{
    public abstract class TabItemModel : INotifyPropertyChanged
    {
        private string _tabCaption = "Tab";
        public string TabCaption { get => _tabCaption; set => SetField(ref _tabCaption, value); }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }

    public class ColumnDefinition
    {
        public string Name { get; set; } = "";
        public string DataType { get; set; } = "";
        public bool IsNullable { get; set; }
    }
}