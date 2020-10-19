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
using System.Windows.Shapes;

namespace MoniteerConsole
{
    /// <summary>
    /// Interaction logic for ClientActionMenu.xaml
    /// </summary>
    public partial class ClientActionMenu : Window
    {

        private int clientId;
        private string machineName;

        public ClientActionMenu(int _clientId)
        {
            clientId = _clientId;
            machineName = App.console.clients[clientId];

            InitializeComponent();
            this.Title = "Perform an action on " + machineName;
        }

        private void sendMsg_button_click(object sender, RoutedEventArgs e)
        {
            App.NotYetImplemented();
        }

        private void openChat_button_click(object sender, RoutedEventArgs e)
        {
            App.NotYetImplemented();
        }

        private void doADab_button_click(object sender, RoutedEventArgs e)
        {
            App.NotYetImplemented();
        }
    }
}
