using MaterialDesignThemes.Wpf;
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
    public partial class RemoveControllerDialog : UserControl
    {
        List<Controller> controllerList = new List<Controller>();
        Action refreshControllerStatus, closeDialog;
        public RemoveControllerDialog(List<Controller> controllerList, Action refreshControllerStatus, Action closeDialog)
        {
            this.controllerList = controllerList;
            this.refreshControllerStatus = refreshControllerStatus;
            this.closeDialog = closeDialog;

            InitializeComponent();

            ControllerTypeBox.Items.Add(Controller.ControllerType.JoyCon);
            ControllerTypeBox.Items.Add(Controller.ControllerType.ProController);
            ControllerTypeBox.Items.Add(Controller.ControllerType.GameCube);
            ControllerTypeBox.Items.Add(Controller.ControllerType.SwitchConsole);
            ControllerTypeBox.Items.Add(Controller.ControllerType.XboxWireless);
            ControllerTypeBox.Items.Add(Controller.ControllerType.XboxWired);
            ControllerTypeBox.Items.Add(Controller.ControllerType.Mouse);
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
                int numOfController = 0;

                //Grab the selected controller type and then clear the controller name ComboBox
                var controllerTypeItem = (Controller.ControllerType)ControllerTypeBox.SelectedItem;
                ControllerNumberBox.Items.Clear();

                foreach (Controller controller in GetControllersOfOneType(controllerTypeItem))
                {
                    //Set the hint to name
                    HintAssist.SetHint(ControllerNumberBox, "Name");

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
                    statusLight.VerticalAlignment = VerticalAlignment.Center;
                    statusLight.Margin = new Thickness(0, 0, 5, 0);

                    //Create the TextBlock for the controller name
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = controller.name;

                    //Add the StatusLight and Name to the stack panel
                    panel.Children.Add(statusLight);
                    panel.Children.Add(textBlock);

                    //Add the StackPanel to the ComboBoxItem and then add it to the ComboBox
                    newItem.Content = panel;
                    ControllerNumberBox.Items.Add(newItem);
                    numOfController++;
                }

                if (numOfController == 0)
                {
                    //Change the hint to say no items found
                    HintAssist.SetHint(ControllerNumberBox, "No Item Found");
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

                    //Refresh the MainWindow's NumberComboBox
                    Dispatcher.Invoke(refreshControllerStatus);

                    //Refresh the NumberComboBox
                    RefreshControllerStatus();

                    //Remove the csv file associated with the controller
                    File.Delete(currController.historyFile);

                    //Close the dialog
                    Dispatcher.Invoke(closeDialog);
                }
                else
                {
                    ValidityBox.Foreground = Brushes.Red;
                    ValidityBox.Text = "Controller doesn't exist";
                }
            }
            else
            {
                ValidityBox.Foreground = Brushes.Red;
                ValidityBox.Text = "Controller doesn't exist";
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            //Close the dialog
            Dispatcher.Invoke(closeDialog);
        }

        private void ControllerNumberBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
