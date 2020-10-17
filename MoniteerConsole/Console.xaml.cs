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
    public partial class Console : Window
    {
        public Console()
        {

            Login login = Window.GetWindow(this) as Login;
            if (login != null)
                login.Close();

            var splash = new SplashScreen("splash.png");
            splash.Show(false);

            InitializeComponent();

            splash.Close(TimeSpan.FromSeconds(0));
        }
    }
}
