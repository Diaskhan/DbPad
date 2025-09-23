using Avalonia.Controls;
using DbPad.ViewModels;

namespace DbPad.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}