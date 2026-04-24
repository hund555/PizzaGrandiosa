using System.Windows;
using PizzaWPF.ViewModels;

namespace PizzaWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}