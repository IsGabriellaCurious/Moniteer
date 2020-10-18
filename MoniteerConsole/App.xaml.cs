using MoniteerClient;
using MoniteerLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MoniteerConsole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static ClientService clientService;
        public static SplashScreen startSplash;
        public static Login login;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            startSplash = new SplashScreen("splash.png");
            startSplash.Show(false);

            clientService = new ClientService();
            clientService.client.consoleHandlers = new Dictionary<int, Constants.ConsoleHandler>()
            {
                { (int)ServerPackets.passwordCheckResponse, ConsoleHandler.PasswordCheckResponse }
            };
            clientService.Start(true);

            Current.Dispatcher.Invoke((Action)(() =>
            {
                login = new Login();
                login.Show();
            }));
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            clientService.Stop();
        }
    }
}
