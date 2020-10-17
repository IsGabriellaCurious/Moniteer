using MoniteerLib;
using MoniteerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoniteerConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            App.startSplash.Close(TimeSpan.FromSeconds(0));
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            ClientSend.PasswordCheck(passwordBox.Password.ToString());
        }
    }
}
