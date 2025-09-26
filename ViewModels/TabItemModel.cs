using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DbPad.ViewModels
{
    public class TabItemModel : INotifyPropertyChanged
    {
        private string _tabCaption = "Tab1";
        private string _text1 = "Text1";
        private string _text2 = "Text2";
        private string _database = "";

        public string TabCaption
        {
            get => _tabCaption;
            set => SetField(ref _tabCaption, value);
        }

        public string Text1
        {
            get => _text1;
            set => SetField(ref _text1, value);
        }

        public string Text2
        {
            get => _text2;
            set => SetField(ref _text2, value);
        }
        public string Database
        {
            get => _database;
            set => SetField(ref _database, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
