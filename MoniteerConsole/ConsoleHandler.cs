using MoniteerConsole.errors;
using MoniteerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoniteerConsole
{
    public class ConsoleHandler
    {
        public static void PasswordCheckResponse(Packet _packet)
        {
            bool correct = _packet.ReadBool();

            if (correct)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    App.console = new MConsoleApp();
                    App.console.Show();
                }));
            } 
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ErrorLogin errorLogin = new ErrorLogin();
                    errorLogin.Show();
                }));
            }
        }

        public static void ClientListResponse(Packet _packet)
        {
            App.console.HandleNewClientList(_packet.ReadDictionary());
        }
    }
}
