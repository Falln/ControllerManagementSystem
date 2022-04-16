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
        Action refreshControllerStatus;
        public NewControllerWindow(List<Controller> controllerList, Action refreshControllerStatus)
        {
            this.controllerList = controllerList;
            this.refreshControllerStatus = refreshControllerStatus;
            InitializeComponent();

            TypeBox.Items.Add(Controller.ControllerType.Switch);
            TypeBox.Items.Add(Controller.ControllerType.Xbox);
            TypeBox.Items.Add(Controller.ControllerType.Other);
        }

        private void AddControllerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TypeBox.SelectedItem != null && ConditionBox.SelectedItem != null)
            {
                Controller.ControllerType controllerType = (Controller.ControllerType)TypeBox.SelectedItem;
                ComboBoxItem controllerCondition = (ComboBoxItem)ConditionBox.SelectedItem;
                Controller newController = new Controller(NameBox.Text, controllerType, controllerCondition.Content.ToString());
                controllerList.Add(newController);
                controllerList.Sort();
                Dispatcher.Invoke(refreshControllerStatus);
            }   
        }
    }
}
