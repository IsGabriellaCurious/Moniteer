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

namespace MoniteerConsole
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class MConsoleApp : Window
    {

        public Dictionary<int, string> clients;

        public MConsoleApp()
        {
            App.login.Close();

            var splash = new SplashScreen("splash.png");
            splash.Show(false);

            clients = new Dictionary<int, string>();
            ClientSend.ClientList();

            InitializeComponent();

            splash.Close(TimeSpan.FromSeconds(0));
        }

        public void HandleNewClientList(Dictionary<int, string> _clients)
        {

            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                clients = _clients;
                clientListPannel.Children.Clear();
                foreach (KeyValuePair<int, string> entry in clients)
                {
                    int id = entry.Key;
                    string name = entry.Value;
                    Button button = new Button
                    {
                        Content = name
                    };
                    button.AddHandler(Button.MouseDoubleClickEvent, new RoutedEventHandler(OpenActionMenuButton));

                    clientListPannel.Children.Add(button);
                }
                if (clientListPannel.Children.Count == 0)
                {
                    clientListPannel.Children.Add(new TextBlock { Text = "No clients found.", HorizontalAlignment = HorizontalAlignment.Center });
                }
            }));
        }

        private void OpenActionMenuButton(object sender, RoutedEventArgs e)
        {
            string machine = (e.Source as Button).Content.ToString();
            int id = clients.FirstOrDefault(x => x.Value == machine).Key; //this shit heavily relies on machines not having the same name lol

            ClientActionMenu cam = new ClientActionMenu(id);
            cam.Show();
        }

        private void refreshClientsButton_Click(object sender, RoutedEventArgs e)
        {
            ClientSend.ClientList();
        }
    }
}
