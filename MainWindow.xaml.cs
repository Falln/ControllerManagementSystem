using Sky.Data.Csv;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControllerManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public class ControllerResolver : AbstractDataResolver<Controller>
    {
        public override Controller Deserialize(List<String> data)
        {
            return new Controller(data[0], Controller.FromStringToControllerType(data[1]), (data[4]=="Checked Out") ? true:false, data[6]);
        }

        public override List<String> Serialize(Controller data)
        {
            return new List<String>
            {
                data.name,
                data.controllerType.ToString(),
            };
        }

        
    }


    public partial class MainWindow : Window
    {
        //List of Controllers
        List<Controller> controllerList = new();

        public MainWindow()
        {
            //Create the controller folder if it doesnt exist
            string controllerCSVDirectory = "Controller CSV Files";
            System.IO.Directory.CreateDirectory(controllerCSVDirectory);

            //Adding test controllers
            /*
            controllerList.Add(new Controller("Joycon1", ControllerType.Switch));
            controllerList.Add(new Controller("Joycon2", ControllerType.Switch));
            controllerList.Add(new Controller("ProController1", ControllerType.Switch));
            controllerList.Add(new Controller("ProController2", ControllerType.Switch));
            controllerList.Add(new Controller("Wired1", ControllerType.Xbox));
            controllerList.Add(new Controller("Wired2", ControllerType.Xbox));
            controllerList.Add(new Controller("Wireless1", ControllerType.Xbox));
            controllerList.Add(new Controller("Mouse1", ControllerType.Other));
            controllerList.Add(new Controller("Keyboard2", ControllerType.Other));
            controllerList.Add(new Controller("Keyboard13", ControllerType.Other));
            controllerList.Add(new Controller("Keyboard11", ControllerType.Other));
            controllerList.Add(new Controller("Mouse2", ControllerType.Other));
            controllerList.Add(new Controller("Mouse23", ControllerType.Other));
            controllerList.Add(new Controller("Mouse14", ControllerType.Other));
            */

            //Load controllers frome the controller CSV folder to the controller List<> and then sort the List
            LoadControllers(controllerCSVDirectory);
            controllerList.Sort();

            InitializeComponent();

            //Add ControllerTypes to the Type ComboBox
            ControllerTypeBox.Items.Add(Controller.ControllerType.Switch);
            ControllerTypeBox.Items.Add(Controller.ControllerType.Xbox);
            ControllerTypeBox.Items.Add(Controller.ControllerType.Other);
        }

        public List<Controller> LoadControllers(string controllerCSVDirectory)
        {
            //Grab all the Controller files
            string[] controllerCSVFiles = System.IO.Directory.GetFiles(controllerCSVDirectory);

            //Loop through each controller files and load the controller using the CSV data
            foreach (string controllerCSVFile in controllerCSVFiles)
            {
                //Start the CSV file reader
                var dataResolver = new ControllerResolver();
                using (var reader = CsvReader<Controller>.Create(controllerCSVFile, dataResolver))
                {
                    var baseController = reader.Last();
                    controllerList.Add(baseController);
                }
            }
            return controllerList;
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

        //Clicks
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewControllerWindow window = new NewControllerWindow(controllerList);
            window.Show();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void minimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void makeResizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowStyle.Equals(WindowStyle.SingleBorderWindow))
            {
                this.WindowStyle = WindowStyle.None;
                this.Height = 850;
                this.Width = 1920;
                this.Left = 0;
                this.Top = 0;
                this.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.ResizeMode = ResizeMode.CanResize;
            }
        }

        private void ControllerTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Grab the selected controller type and then clear the controller name ComboBox
            var controllerTypeItem = (Controller.ControllerType)ControllerTypeBox.SelectedItem;
            ControllerNumberBox.Items.Clear();

            if (controllerTypeItem == Controller.ControllerType.Switch)
            {
                foreach (Controller controller in GetControllersOfOneType(Controller.ControllerType.Switch))
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
            //If its an xbox controller, add all xbox controllers to the name ComboBox
            else if (controllerTypeItem == Controller.ControllerType.Xbox)
            {
                foreach (Controller controller in GetControllersOfOneType(Controller.ControllerType.Xbox))
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
            //If its an other item, add all other items to the name ComboBox
            else if (controllerTypeItem == Controller.ControllerType.Other)
            {
                foreach (Controller controller in GetControllersOfOneType(Controller.ControllerType.Other))
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

        private void ControllerNumberBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBoxItem)ControllerNumberBox.SelectedItem != null)
            {
                //Get the selected controller from the controller list
                Controller.ControllerType controllerTypeItem = (Controller.ControllerType)ControllerTypeBox.SelectedItem;
                ComboBoxItem nameItem = (ComboBoxItem)ControllerNumberBox.SelectedItem;
                StackPanel nameBoxStackPanel = (StackPanel)nameItem.Content;
                TextBlock nameTextBox = (TextBlock)nameBoxStackPanel.Children[1];
                string controllerName = nameTextBox.Text;

                Controller currController = GetController(controllerTypeItem, controllerName);

                //Update test field with the controller
                TestName.Text = currController.name;
                TestStatus.Text = currController.controllerStatus.ToString();
                TestCheckedout.Text = currController.isCheckedOut ? "Checked Out" : "Available";
                TestStatus.Text = currController.controllerStatus;
                TestOwner.Text = currController.currentOwner;
            }
        }


        private void CheckoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((ComboBoxItem)ControllerNumberBox.SelectedItem != null)
            {
                //Get the currently selected name and controller type, and then check that controller out
                Controller.ControllerType controllerTypeItem = (Controller.ControllerType)ControllerTypeBox.SelectedItem;
                ComboBoxItem nameItem = (ComboBoxItem)ControllerNumberBox.SelectedItem;
                StackPanel nameBoxStackPanel = (StackPanel)nameItem.Content;
                TextBlock nameTextBox = (TextBlock)nameBoxStackPanel.Children[1];
                string controllerName = nameTextBox.Text;

                Controller currController = GetController(controllerTypeItem, controllerName);
                currController.checkOut(UsernameCheckoutBox.Text, "New", InitialsBox.Text);

                //Update all ComboBoxes with the new status
                int previousIndex = ControllerNumberBox.SelectedIndex;
                ControllerTypeBox_SelectionChanged(new object(), null!);
                ControllerNumberBox.SelectedIndex = previousIndex;
            }
            
        }

        private void CheckinBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((ComboBoxItem)ControllerNumberBox.SelectedItem != null)
            {
                //Get the currently selected name and controller type, and then check that controller in
                Controller.ControllerType controllerTypeItem = (Controller.ControllerType)ControllerTypeBox.SelectedItem;
                ComboBoxItem nameItem = (ComboBoxItem)ControllerNumberBox.SelectedItem;
                StackPanel nameBoxStackPanel = (StackPanel)nameItem.Content;
                TextBlock nameTextBox = (TextBlock)nameBoxStackPanel.Children[1];
                string controllerName = nameTextBox.Text;

                Controller currController = GetController(controllerTypeItem, controllerName);
                currController.checkIn(UsernameCheckoutBox.Text, "New", InitialsBox.Text);


                //Update all ComboBoxes with the new status
                int previousIndex = ControllerNumberBox.SelectedIndex;
                ControllerTypeBox_SelectionChanged(new object(), null!);
                ControllerNumberBox.SelectedIndex = previousIndex;
            }
        }
    }
}
