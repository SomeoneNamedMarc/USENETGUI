using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NNTPGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel(new UseNetConnection());
            DataContext = ViewModel;

        }

        private void OpenConnectionWindow_Click(object sender, RoutedEventArgs e)
        {
            var connectionWindow = new ConnectionWindow(ViewModel);
            connectionWindow.ShowDialog();
        }
    }
}