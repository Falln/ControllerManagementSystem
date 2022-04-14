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

namespace ControllerManagementSystem
{
    /// <summary>
    /// Interaction logic for NewControllerWindow.xaml
    /// </summary>
    public partial class NewControllerWindow : Window
    {
        List<Controller> controllerList = new();
        public NewControllerWindow(List<Controller> controllerList)
        {
            this.controllerList = controllerList;
            InitializeComponent();
        }
    }
}
