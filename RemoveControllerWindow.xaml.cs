using AdonisUI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ControllerManagementSystem
{
    /// <summary>
    /// Interaction logic for RemoveControllerWindow.xaml
    /// </summary>
    public partial class RemoveControllerWindow : AdonisWindow
    {
        List<Controller> controllerList = new List<Controller>();
        Action refreshControllerStatus;
        public RemoveControllerWindow(List<Controller> controllerList, Action refreshControllerStatus)
        {
            this.controllerList = controllerList;
            this.refreshControllerStatus = refreshControllerStatus;
            
            InitializeComponent();

            ControllerTypeBox.Items.Add(Controller.ControllerType.Switch);
            ControllerTypeBox.Items.Add(Controller.ControllerType.Xbox);
            ControllerTypeBox.Items.Add(Controller.ControllerType.Other);

            //Set the default selected item for the TypeBox
            ControllerTypeBox.SelectedIndex = 0;
        }


        public List<Controller> GetControllersOfOneType(Controller.ControllerType controllerType)
        {
            List<Controller> newControllerList = new();
            foreach (Controller controller in controllerList)
            {
                if (controller.controllerType == controllerType)
                {
                    newControllerList.Add(controller);
                }
            }
            return newControllerList;
        }

        public Controller GetController(Controller.ControllerType controllerType, string name)
        {
            foreach (Controller controller in controllerList)
            {
                if (controller.controllerType.Equals(controllerType) && controller.name.Equals(name))
                {
                    return controller;
                }
            }
            return new Controller();
        }

        public void RefreshControllerStatus()
        {
            int previousIndex = ControllerNumberBox.SelectedIndex;
            ControllerTypeBox_SelectionChanged(new object(), null!);
            ControllerNumberBox.SelectedIndex = previousIndex;
        }

        private void ControllerTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ControllerTypeBox.SelectedItem != null)
            {
                //Grab the selected controller type and then clear the controller name ComboBox
                var controllerTypeItem = (Controller.ControllerType)ControllerTypeBox.SelectedItem;
                ControllerNumberBox.Items.Clear();

                foreach (Controller controller in GetControllersOfOneType(controllerTypeItem))
                {
                    //Create the ComboBox and the StackPanel to go in it
                    ComboBoxItem newItem = new ComboBoxItem();
                    StackPanel panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    //Create the Ellipse and the BulletDecorator it goes inside. This is the "Checked out" StatusLight
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = controller.isCheckedOut ? Brushes.Red : Brushes.Green;
                    ellipse.Stroke = Brushes.Black;
                    ellipse.StrokeThickness = 0;
                    ellipse.Height = 5;
                    ellipse.Width = 5;
                    BulletDecorator statusLight = new BulletDecorator();
                    statusLight.Bullet = ellipse;
                    statusLight.Margin = new Thickness(0, 5, 5, 0);

                    //Create the TextBlock for the controller name
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = controller.name;

                    //Add the StatusLight and Name to the stack panel
                    panel.Children.Add(statusLight);
                    panel.Children.Add(textBlock);

                    //Add the StackPanel to the ComboBoxItem and then add it to the ComboBox
                    newItem.Content = panel;
                    ControllerNumberBox.Items.Add(newItem);
                }
                ControllerNumberBox.SelectedIndex = 0;
            }
        }

        private void RemoveControllerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ControllerNumberBox.SelectedItem != null)
            {
                //Get the selected controller from the controller list
                Controller.ControllerType controllerTypeItem = (Controller.ControllerType)ControllerTypeBox.SelectedItem;
                ComboBoxItem nameItem = (ComboBoxItem)ControllerNumberBox.SelectedItem;
                StackPanel nameBoxStackPanel = (StackPanel)nameItem.Content;
                TextBlock nameTextBox = (TextBlock)nameBoxStackPanel.Children[1];
                string controllerName = nameTextBox.Text;

                Controller currController = GetController(controllerTypeItem, controllerName);

                //Remove the controller and output whether it was successfully removed
                bool controllerRemoved = controllerList.Remove(currController);
                if (controllerRemoved)
                {
                    ValidityBox.Foreground = Brushes.Green;
                    ValidityBox.Text = "Controller removed sucessfully";
                }
                else
                {
                    ValidityBox.Foreground = Brushes.Red;
                    ValidityBox.Text = "Controller doesn't exist";
                }

                //Refresh the MainWindow's NumberComboBox
                Dispatcher.Invoke(refreshControllerStatus);

                //Refresh the NumberComboBox
                RefreshControllerStatus();

                //Remove the csv file associated with the controller
                File.Delete(currController.historyFile);
            }
        }

        private void ControllerNumberBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
