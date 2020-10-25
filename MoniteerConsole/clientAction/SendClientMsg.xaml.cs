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
using System.Windows.Shapes;

namespace MoniteerConsole.clientAction
{
    /// <summary>
    /// Interaction logic for SendClientMsg.xaml
    /// </summary>
    public partial class SendClientMsg : Window
    {

        private int clientId;
        private string machineName;
        private ClientActionMenu cam;

        public SendClientMsg(int _clientId, string _machineName, ClientActionMenu _cam)
        {
            clientId = _clientId;
            machineName = _machineName;
            cam = _cam;
            InitializeComponent();
            this.Title = "Send message to " + machineName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClientSend.ConsoleMsgSend(clientId, msgToSend.Text, App.loggedInUser);
            cam.Close();
            Close();
        }
    }
}
