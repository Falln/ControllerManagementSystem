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
    public enum ControllerType
    {
        Switch,
        Joycon,
        ProController,
        Xbox,
        XboxWireless,
        XboxWired,
        Other
    }

    public partial class Controller : IComparable
    {
        public readonly ControllerType controllerType;
        public readonly string name;
        public string controllerStatus;
        public string currentOwner = "";
        public Boolean isCheckedOut = false;
        public string historyFile;

        public Controller()
        {
            controllerType = ControllerType.Other;
            name = "";
            controllerStatus = "";
            historyFile = "";
        }

        public Controller(string name, ControllerType controllerType)
        {
            this.name = name;
            this.controllerType = controllerType;
            this.controllerStatus = "New";
            historyFile = "Controller CSV Files/" + name + " History CSV.csv";
            try
            {
                //Check if the files already exists, if so, don't create a new file
                if (!File.Exists(historyFile))
                {
                    //Create a new .csv file
                    using (FileStream fs = File.Create(historyFile)){ }

                    //Write the first entry in the file. This will have time/date and the owner will be CES
                    CsvWriterSettings settings = new();
                    settings.AppendExisting = true;
                    using (var writer = CsvWriter.Create(historyFile, settings))
                    {
                        DateTime now = DateTime.Now;
                        //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                        writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToShortTimeString(), "Checked In", "CES", controllerStatus, "");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                throw e;
            }
        }

        public Controller(string name, ControllerType controllerType, string controllerStatus)
        {
            this.name = name;
            this.controllerType = controllerType;
            this.controllerStatus = controllerStatus;
            historyFile = "Controller CSV Files/" + name + " History CSV.csv";
            try
            {
                //Check if the files already exists, if so, don't create a new file
                if (!File.Exists(historyFile))
                {
                    //Create a new .csv file
                    using (FileStream fs = File.Create(historyFile)) { }

                    //Write the first entry in the file. This will have time/date and the owner will be CES
                    CsvWriterSettings settings = new();
                    settings.AppendExisting = true;
                    using (var writer = CsvWriter.Create(historyFile, settings))
                    {
                        DateTime now = DateTime.Now;
                        //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                        writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToShortTimeString(), "Checked In", "CES", controllerStatus, "");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                throw e;
            }
        }

        public Controller(string name, ControllerType controllerType, Boolean isCheckedOut, string controllerStatus)
        {
            this.name = name;
            this.controllerType = controllerType;
            this.controllerStatus = controllerStatus;
            this.isCheckedOut = isCheckedOut;
            historyFile = "Controller CSV Files/" + name + " History CSV.csv";
            try
            {
                //Check if the files already exists, if so, don't create a new file
                if (!File.Exists(historyFile))
                {
                    //Create a new .csv file
                    using (FileStream fs = File.Create(historyFile)) { }

                    //Write the first entry in the file. This will have time/date and the owner will be CES
                    CsvWriterSettings settings = new();
                    settings.AppendExisting = true;
                    using (var writer = CsvWriter.Create(historyFile, settings))
                    {
                        DateTime now = DateTime.Now;
                        //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                        writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToShortTimeString(), "Checked In", "CES", controllerStatus, "");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                throw e;
            }
        }

        public void setCheckedOut(Boolean isCheckedOut, string owner, string controllerStatus)
        {
            //Set the controller checked out status to the one given
            this.isCheckedOut = isCheckedOut;

            //Start the CSV writer
            CsvWriterSettings settings = new();
            settings.AppendExisting = true;
            using (var writer = CsvWriter.Create(historyFile, settings))
            {
                DateTime now = DateTime.Now;
                //Add a CSV entry with the checked out status as the one given
                //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToShortTimeString(), isCheckedOut ? "Checked Out":"Checked In", owner, controllerStatus, "");
            }
        }

        public void checkOut(string owner, string controllerStatus)
        {
            //Set the controller to checked out
            this.isCheckedOut = true;

            //Start the CSV writer
            CsvWriterSettings settings = new();
            settings.AppendExisting = true;
            using (var writer = CsvWriter.Create(historyFile, settings))
            {
                DateTime now = DateTime.Now;
                //Add a CSV entry of "checked in"
                //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToShortTimeString(), "Checked Out", owner, controllerStatus, "");
            }
        }

        public void checkIn(string owner, string controllerStatus)
        {
            //Set the controller to checked in
            this.isCheckedOut = false;

            //Start the CSV writer
            CsvWriterSettings settings = new();
            settings.AppendExisting = true;
            using (var writer = CsvWriter.Create(historyFile, settings))
            {
                DateTime now = DateTime.Now;
                //Add a CSV entry of "checked in"
                //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToShortTimeString(), "Checked In", owner, controllerStatus, "");
            }
        }


        public int CompareTo(Controller obj)
        {
            double thisControllerNum = Double.Parse(new string(this.name.Where(Char.IsDigit).ToArray()));
            double newControllerNum = Double.Parse(new string(obj.name.Where(Char.IsDigit).ToArray()));
            if (this.name.Length < obj.name.Length)
                return -1;
            else if (this.name.Length > obj.name.Length)
                return 1;
            else if (thisControllerNum < newControllerNum)
                return -1;
            else if (thisControllerNum > newControllerNum)
                return 1;
            else if (thisControllerNum == newControllerNum && this.name.Length == obj.name.Length)
                return 0;
            else
                throw new ArgumentException("Object is not a Controller");
        }

        public int CompareTo(Object? obj)
        {
            try
            {
                return CompareTo(obj as Controller);
            } catch (Exception ex)
            {
                throw new ArgumentException("Object is not a Controller or is null");
            }
        }
    }

    public class ControllerResolver : AbstractDataResolver<Controller>
    {
        public override Controller Deserialize(List<String> data)
        {
            return new Controller(data[0], FromStringToControllerType(data[1]), (data[4]=="Checked Out") ? true:false, data[6]);
        }

        public override List<String> Serialize(Controller data)
        {
            return new List<String>
            {
                data.name,
                data.controllerType.ToString(),
            };
        }

        private ControllerType FromStringToControllerType(string controllerTypeString)
        {
            switch (controllerTypeString)
            {
                case "Switch":
                    return ControllerType.Switch;
                case "Xbox":
                    return ControllerType.Xbox;
                case "Other":
                    return ControllerType.Other;
                case "ProController":
                    return ControllerType.ProController;
                case "Joycon":
                    return ControllerType.Joycon;
                case "XboxWireless":
                    return ControllerType.XboxWireless;
                case "XboxWired":
                    return ControllerType.XboxWired;
                default:
                    return ControllerType.Other;
            }
        }
    }



    public partial class MainWindow : Window
    {
        List<Controller> controllerList = new();

        public MainWindow()
        {

            string controllerCSVDirectory = "Controller CSV Files";
            System.IO.Directory.CreateDirectory(controllerCSVDirectory);
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
            LoadControllers(controllerCSVDirectory);

            controllerList.Sort();
            InitializeComponent();

            ControllerTypeBox.Items.Add(ControllerType.Switch);
            ControllerTypeBox.Items.Add(ControllerType.Xbox);
            ControllerTypeBox.Items.Add(ControllerType.Other);
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

        public List<Controller> GetControllersOfOneType(ControllerType controllerType)
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

        public Controller GetController(ControllerType controllerType, string name)
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
            ControllerType controllerTypeItem = (ControllerType)ControllerTypeBox.SelectedItem;
            ControllerNumberBox.Items.Clear();

            if (controllerTypeItem == ControllerType.Switch)
            {
                foreach (Controller controller in GetControllersOfOneType(ControllerType.Switch))
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
            else if (controllerTypeItem == ControllerType.Xbox)
            {
                foreach (Controller controller in GetControllersOfOneType(ControllerType.Xbox))
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
            else if (controllerTypeItem == ControllerType.Other)
            {
                foreach (Controller controller in GetControllersOfOneType(ControllerType.Other))
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
                //Get the currently selected name and controller type, and then check that controller out
                ControllerType controllerTypeItem = (ControllerType)ControllerTypeBox.SelectedItem;
                ComboBoxItem nameItem = (ComboBoxItem)ControllerNumberBox.SelectedItem;
                StackPanel nameBoxStackPanel = (StackPanel)nameItem.Content;
                TextBlock nameTextBox = (TextBlock)nameBoxStackPanel.Children[1];
                string controllerName = nameTextBox.Text;

                Controller currController = GetController(controllerTypeItem, controllerName);

                //Update test field
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
                ControllerType controllerTypeItem = (ControllerType)ControllerTypeBox.SelectedItem;
                ComboBoxItem nameItem = (ComboBoxItem)ControllerNumberBox.SelectedItem;
                StackPanel nameBoxStackPanel = (StackPanel)nameItem.Content;
                TextBlock nameTextBox = (TextBlock)nameBoxStackPanel.Children[1];
                string controllerName = nameTextBox.Text;

                Controller currController = GetController(controllerTypeItem, controllerName);
                currController.checkOut(UsernameCheckoutBox.Text, "New");

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
                //Get the currently selected name and controller type, and then check that controller out
                ControllerType controllerTypeItem = (ControllerType)ControllerTypeBox.SelectedItem;
                ComboBoxItem nameItem = (ComboBoxItem)ControllerNumberBox.SelectedItem;
                StackPanel nameBoxStackPanel = (StackPanel)nameItem.Content;
                TextBlock nameTextBox = (TextBlock)nameBoxStackPanel.Children[1];
                string controllerName = nameTextBox.Text;

                Controller currController = GetController(controllerTypeItem, controllerName);
                currController.checkIn(UsernameCheckoutBox.Text, "New");


                //Update all ComboBoxes with the new status
                int previousIndex = ControllerNumberBox.SelectedIndex;
                ControllerTypeBox_SelectionChanged(new object(), null!);
                ControllerNumberBox.SelectedIndex = previousIndex;
            }
        }
    }
}
