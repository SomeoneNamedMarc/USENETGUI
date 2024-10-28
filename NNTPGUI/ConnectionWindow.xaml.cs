using System;
using System.Windows;

namespace NNTPGUI
{
    public partial class ConnectionWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public ConnectionWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string host = HostTextBox.Text;
            if (!int.TryParse(PortTextBox.Text, out int port))
            {
                MessageBox.Show("Invalid port number");
                return;
            }
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Text;

            await _mainViewModel.InitializeAsync(host, port, username, password);

            this.Close();
        }
    }
}