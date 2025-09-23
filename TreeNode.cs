using ReactiveUI;
using System.Collections.ObjectModel;

namespace DbPad.ViewModels
{
    public class TreeNode : ReactiveObject
    {
        public string Title { get; set; }
        public ObservableCollection<TreeNode> Children { get; } = new();

        public TreeNode(string title)
        {
            Title = title;
        }
    }
}