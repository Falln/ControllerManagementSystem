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

namespace ControllerManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum ControllerType
        {
            Switch,
            Joycon,
            ProController,
            Xbox,
            XboxWireless,
            XboxWired,
            Other
        }

        private class Controller
        {
            public readonly ControllerType controllerType;
            public readonly string name;

            public Controller(string name, ControllerType controllerType)
            {
                this.name = name;
                this.controllerType = controllerType;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
