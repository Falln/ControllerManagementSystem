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

            TypeBox.Items.Add(Controller.ControllerType.JoyCon);
            TypeBox.Items.Add(Controller.ControllerType.ProController);
            TypeBox.Items.Add(Controller.ControllerType.GameCube);
            TypeBox.Items.Add(Controller.ControllerType.SwitchConsole);
            TypeBox.Items.Add(Controller.ControllerType.XboxWireless);
            TypeBox.Items.Add(Controller.ControllerType.XboxWired);
            TypeBox.Items.Add(Controller.ControllerType.Mouse);
            TypeBox.Items.Add(Controller.ControllerType.Other);
        }

        private void AddControllerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (checkControllerValidity())
            {
                Controller.ControllerType controllerType = (Controller.ControllerType)TypeBox.SelectedItem;
                ComboBoxItem controllerCondition = (ComboBoxItem)ConditionBox.SelectedItem;
                Controller newController = new Controller(NameBox.Text.ToString(), controllerType, controllerCondition.Content.ToString());
                controllerList.Add(newController);
                controllerList.Sort();
                Dispatcher.Invoke(refreshControllerStatus);
            }
        }

        private bool checkControllerValidity()
        {
            if (TypeBox.SelectedItem == null)
            {
                ValidityBox.Foreground = Brushes.Red;
                ValidityBox.Text = "Please enter controller type";
                return false;
            }
            else if (NameBox.Text == "" || NameBox.Text == null || NameBox.Text == " ")
            {
                ValidityBox.Foreground = Brushes.Red;
                ValidityBox.Text = "Please enter valid name";
                return false;
            }
            else if (ConditionBox.SelectedItem == null)
            {
                ValidityBox.Foreground = Brushes.Red;
                ValidityBox.Text = "Please enter controller condition";
                return false;
            } 
            else if (Controller.ContainsSameName(controllerList, NameBox.Text))
            {
                ValidityBox.Foreground = Brushes.Red;
                ValidityBox.Text = "Controller name already exists";
                return false;
            } 
            else
            {
                ValidityBox.Foreground = Brushes.Green;
                ValidityBox.Text = "Controller Added";
                return true;
            }
        }

        private void ClearValidityBox(object sender, RoutedEventArgs e)
        {
            //Reset the validity box when a new type is selected
            ValidityBox.Text = "";
        }
    }
}
