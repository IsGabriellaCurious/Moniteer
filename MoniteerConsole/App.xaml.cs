using MoniteerClient;
using MoniteerConsole.errors;
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
        public static MConsoleApp console;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            startSplash = new SplashScreen("splash.png");
            startSplash.Show(false);

            clientService = new ClientService();
            clientService.client.consoleHandlers = new Dictionary<int, Constants.ConsoleHandler>()
            {
                { (int)ServerPackets.passwordCheckResponse, ConsoleHandler.PasswordCheckResponse },
                { (int)ServerPackets.clientListResponse, ConsoleHandler.ClientListResponse }
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

        public static void NotYetImplemented()
        {
            Current.Dispatcher.Invoke((Action)(() =>
            {
                FNYIError err = new FNYIError();
                err.Show();
            }));
        }
    }
}
