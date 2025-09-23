using DbPad.Models;
using System.Collections.ObjectModel;

namespace DbPad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Node> Nodes { get; }

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
        }
    }
}
