using DbPad.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DbPad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Node> Nodes { get; }
        public ObservableCollection<TabItemModel> Tabs { get; }
        public ICommand AddTabCommand { get; }

        public MainWindowViewModel()
        {
            Nodes = new ObservableCollection<Node>
            {
                new Node("Animals", new ObservableCollection<Node>
                {
                    new Node("Mammals", new ObservableCollection<Node>
                    {
                        new Node("Lion"), new Node("Cat"), new Node("Zebra")
                    })
                })
            };

            Tabs = new ObservableCollection<TabItemModel>();
            AddTabCommand = new RelayCommand(AddTab);
        }

        private void AddTab()
        {
            Tabs.Add(new TabItemModel());
        }
    }
}
