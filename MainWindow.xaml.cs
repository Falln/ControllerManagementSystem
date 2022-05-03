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
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using MaterialDesignExtensions.Controls;
using System.Globalization;

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
            return new Controller(data[0], Controller.FromStringToControllerType(data[1]), (data[4] == "Checked Out") ? true : false, data[6], data[5]);
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

    public class BaseCSVResolver : AbstractDataResolver<List<string>>
    {
        public override List<string> Deserialize(List<string> data)
        {
            return data;
        }

        public override List<string> Serialize(List<string> data)
        {
            return data;
        }
    }

    public class ControllerHistroyResolver : AbstractDataResolver<Controller.HistoryEntry>
    {
        public override Controller.HistoryEntry Deserialize(List<string> data)
        {

            return new Controller.HistoryEntry
            {
                name = data[0],
                controllerType = Controller.FromStringToControllerType(data[1]),
                dateTime = DateTime.Parse(data[2] + " " + data[3]),
                checkedStatus = data[4],
                currentOwner = data[5],
                condition = data[6],
                initials = data[7],
            };
        }

        public override List<string> Serialize(Controller.HistoryEntry data)
        {
            return new List<string>
            {
                data.name,
                data.controllerType.ToString(),
                data.dateTime.ToShortDateString(),
                data.dateTime.ToShortTimeString(),
                data.checkedStatus,
                data.currentOwner,
                data.condition,
                data.initials
            };
        }
    }


    public partial class MainWindow : MaterialWindow
    {
        //List of Controllers
        List<Controller> controllerList = new();

        //Add/Remove Windows
        NewControllerWindow newControllerWindow;
        RemoveControllerWindow removeControllerWindow;

        public MainWindow()
        {
            //Create the controller folder if it doesnt exist
            string controllerCSVDirectory = "Controller CSV Files";
            System.IO.Directory.CreateDirectory(controllerCSVDirectory);

            //Adding test controllers
            /*
            controllerList.Add(new Controller("JoyCon1", Controller.ControllerType.JoyCon));
            controllerList.Add(new Controller("JoyCon2", Controller.ControllerType.JoyCon));
            controllerList.Add(new Controller("ProController1", Controller.ControllerType.ProController));
            controllerList.Add(new Controller("ProController2", Controller.ControllerType.ProController));
            controllerList.Add(new Controller("Wired1", Controller.ControllerType.XboxWired));
            controllerList.Add(new Controller("Wired2", Controller.ControllerType.XboxWired));
            controllerList.Add(new Controller("Wireless1", Controller.ControllerType.XboxWireless));
            controllerList.Add(new Controller("Mouse1", Controller.ControllerType.Mouse));
            controllerList.Add(new Controller("Keyboard2", Controller.ControllerType.Other));
            controllerList.Add(new Controller("Keyboard13", Controller.ControllerType.Other));
            controllerList.Add(new Controller("Keyboard11", Controller.ControllerType.Other));
            controllerList.Add(new Controller("Mouse2", Controller.ControllerType.Mouse));
            controllerList.Add(new Controller("Mouse23", Controller.ControllerType.Mouse));
            controllerList.Add(new Controller("Mouse14", Controller.ControllerType.Mouse));
            */

            //Load controllers frome the controller CSV folder to the controller List<> and then sort the List
            LoadControllers(controllerCSVDirectory);
            controllerList.Sort();

            InitializeComponent();
            

            //TODO if adding more Controller types, add things here
            //Add ControllerTypes to the Type ComboBoxes
            ControllerTypeBox.Items.Add(Controller.ControllerType.JoyCon);
            ControllerTypeBox.Items.Add(Controller.ControllerType.ProController);
            ControllerTypeBox.Items.Add(Controller.ControllerType.SwitchConsole);
            ControllerTypeBox.Items.Add(Controller.ControllerType.XboxWireless);
            ControllerTypeBox.Items.Add(Controller.ControllerType.XboxWired);
            ControllerTypeBox.Items.Add(Controller.ControllerType.Mouse);
            ControllerTypeBox.Items.Add(Controller.ControllerType.Other);

            ItemHistTypeBox.Items.Add(Controller.ControllerType.JoyCon);
            ItemHistTypeBox.Items.Add(Controller.ControllerType.ProController);
            ItemHistTypeBox.Items.Add(Controller.ControllerType.SwitchConsole);
            ItemHistTypeBox.Items.Add(Controller.ControllerType.XboxWireless);
            ItemHistTypeBox.Items.Add(Controller.ControllerType.XboxWired);
            ItemHistTypeBox.Items.Add(Controller.ControllerType.Mouse);
            ItemHistTypeBox.Items.Add(Controller.ControllerType.Other);

            //Set the default selected item for the TypeBox
            ControllerTypeBox.SelectedIndex = 0;

            //Update stuff from settings
            string testcolor = Properties.Settings.Default.primaryColor;
            Color primaryColor = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.primaryColor);

            //Set the theme based on the one kept in settings
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            theme.SetPrimaryColor(primaryColor);
            if (Properties.Settings.Default.isThemeDark)
                theme.SetBaseTheme(Theme.Dark);
            else
                theme.SetBaseTheme(Theme.Light);
            paletteHelper.SetTheme(theme);

            //Set the default owner and total # of entries to save
            TotalEntriesBlock.Text = Properties.Settings.Default.totalEntriesToSave.ToString();
            DefaultOwnerBlock.Text = Properties.Settings.Default.defaultOwner;
        }

        private void MaterialWindow_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
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
            if (previousIndex == -1)
                ControllerNumberBox.SelectedIndex = 1;
            else
                ControllerNumberBox.SelectedIndex = previousIndex;

            previousIndex = ItemHistNameBox.SelectedIndex;
            ItemHistTypeBox_SelectionChanged(new object(), null!);
            if (previousIndex == -1)
                ItemHistNameBox.SelectedIndex = 1;
            else
                ItemHistNameBox.SelectedIndex = previousIndex;
        }

        //Clicks
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

        public class QuickViewItem
        {
            public string quickView { get; set; }

            public QuickViewItem(string quickView) { this.quickView = quickView; }
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

                //TODO: Test Variables. Remove later
                //Update test field with the controller
                TestName.Text = currController.name;
                TestType.Text = currController.controllerType.ToString();
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
                RefreshControllerStatus();
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
                currController.checkIn("New", InitialsBox.Text);


                //Update all ComboBoxes with the new status
                RefreshControllerStatus();
            }
        }

        private void AddItemBtn_Click(object sender, RoutedEventArgs e)
        {
            //Check if there are any other add/remove windows open. If so close them
            if (removeControllerWindow != null)
            {
                if (removeControllerWindow.IsEnabled)
                    removeControllerWindow.Close();
            }

            if (newControllerWindow != null)
            {
                if (newControllerWindow.IsEnabled)
                    newControllerWindow.Close();
            }

            //Start new add window
            newControllerWindow = new NewControllerWindow(controllerList, RefreshControllerStatus);
            newControllerWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            newControllerWindow.Left = PointToScreen(Mouse.GetPosition(null)).X;
            newControllerWindow.Top = PointToScreen(Mouse.GetPosition(null)).Y;
            newControllerWindow.Activate();
            newControllerWindow.Show();
        }

        private void RemoveItemBtn_Click(object sender, RoutedEventArgs e)
        {
            //Check if there are any other add/remove windows open. If so close them
            if (removeControllerWindow != null)
            {
                if (removeControllerWindow.IsEnabled)
                    removeControllerWindow.Close();
            }

            if (newControllerWindow != null)
            {
                if (newControllerWindow.IsEnabled)
                    newControllerWindow.Close();
            }

            //Start new remove window
            removeControllerWindow = new RemoveControllerWindow(controllerList, RefreshControllerStatus);
            removeControllerWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            removeControllerWindow.Left = PointToScreen(Mouse.GetPosition(null)).X;
            removeControllerWindow.Top = PointToScreen(Mouse.GetPosition(null)).Y;
            removeControllerWindow.Activate();
            removeControllerWindow.Show();
        }


        private void ItemHistTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemHistTypeBox.SelectedItem != null)
            {
                int numOfController = 0;

                //Grab the selected controller type and then clear the controller name ComboBox
                var controllerTypeItem = (Controller.ControllerType)ItemHistTypeBox.SelectedItem;
                ItemHistNameBox.Items.Clear();

                foreach (Controller controller in GetControllersOfOneType(controllerTypeItem))
                {
                    //Set the hint to name
                    HintAssist.SetHint(ItemHistNameBox, "Name");

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
                    ItemHistNameBox.Items.Add(newItem);
                    numOfController++;
                }

                if (numOfController == 0)
                {
                    //Change the hint to say no items found
                    HintAssist.SetHint(ItemHistNameBox, "No Item Found");
                }

                ItemHistNameBox.SelectedIndex = 0;
            }
        }

        //Check history stuff
        private void ItemHistNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBoxItem)ItemHistNameBox.SelectedItem != null)
            {
                //Get the selected controller from the controller list
                Controller.ControllerType controllerTypeItem = (Controller.ControllerType)ItemHistTypeBox.SelectedItem;
                ComboBoxItem nameItem = (ComboBoxItem)ItemHistNameBox.SelectedItem;
                StackPanel nameBoxStackPanel = (StackPanel)nameItem.Content;
                TextBlock nameTextBox = (TextBlock)nameBoxStackPanel.Children[1];
                string controllerName = nameTextBox.Text;

                Controller currController = GetController(controllerTypeItem, controllerName);
            }
        }


        private void FocusGrid(object sender, EventArgs e)
        {
            Grid.Focus();
        }

        private void CheckHistInsideBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ItemHistTypeBox.SelectedItem != null && ItemHistNameBox.SelectedItem != null)
            {
                //Get the selected controller from the controller list
                Controller.ControllerType controllerTypeItem = (Controller.ControllerType)ItemHistTypeBox.SelectedItem;
                ComboBoxItem nameItem = (ComboBoxItem)ItemHistNameBox.SelectedItem;
                StackPanel nameBoxStackPanel = (StackPanel)nameItem.Content;
                TextBlock nameTextBox = (TextBlock)nameBoxStackPanel.Children[1];
                string controllerName = nameTextBox.Text;

                Controller currController = GetController(controllerTypeItem, controllerName);

                //Get the controller history and store it as a ControllerHistory object in a big list of them
                List<Controller.HistoryEntry> controllerHistoryList = currController.GetControllerHistory();

                //Add the list of the history to the ListView
                DataGrid dataGrid = (DataGrid)CheckHistPopupBox.PopupContent;
                dataGrid.ItemsSource = controllerHistoryList;

                CheckHistPopupBox.IsPopupOpen = true;
            }
        }
        private void ApplyColorBtn_Click(object sender, RoutedEventArgs e)
        {
            //Get the current theme
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            //Change the current accent color to the new accent color
            theme.SetPrimaryColor(ColorPicker.Color);

            //Update the theme and update the settings with the new color
            Properties.Settings.Default.primaryColor = ColorPicker.Color.ToString();
            paletteHelper.SetTheme(theme);

            //Close the color dialog
            ColorDialog.IsOpen = false;
        }

        private void CancelColorBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog.IsOpen = false;
        }

        private void OpenColorDialogBtn_Click(Object sender, RoutedEventArgs e)
        {
            ColorDialog.IsOpen = true;
        }

        private void TotalEntriesBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TotalEntriesBlock.Text != null && TotalEntriesBlock.Text != "")
            {
                Properties.Settings.Default.totalEntriesToSave = int.Parse(TotalEntriesBlock.Text);
            }
        }

        private void TotalEntriesBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DefaultOwnerBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DefaultOwnerBlock.Text == "")
                DefaultOwnerBlock.FontStyle = FontStyles.Italic;
            else
                DefaultOwnerBlock.FontStyle = FontStyles.Normal;
            Properties.Settings.Default.defaultOwner = DefaultOwnerBlock.Text;
        }

        private void ModeTglBtn_Checked(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            BaseTheme theme1 = theme.GetBaseTheme();
            if (theme.GetBaseTheme().Equals(BaseTheme.Light))
            {
                theme.SetBaseTheme(Theme.Dark);
                paletteHelper.SetTheme(theme);
                Properties.Settings.Default.isThemeDark = true;
            }

        }

        private void ModeTglBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            if (theme.GetBaseTheme().Equals(BaseTheme.Dark))
            {
                theme.SetBaseTheme(Theme.Light);
                paletteHelper.SetTheme(theme);
                Properties.Settings.Default.isThemeDark = false;
            }
        }

        private void ColorDialog_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            //Update the ColorSelector
            Color primaryColor = (Color)ColorConverter.ConvertFromString(Properties.Settings.Default.primaryColor);
            string test = (string)new BrushToHexConverter().Convert(new ColorToBrushConverter().Convert(primaryColor, null!, null!, null!), null!, null!, null!);
            ColorPicker.Color = primaryColor;
            ColorPickerHexInput.Text = test;
            ModeTglBtn.IsChecked = Properties.Settings.Default.isThemeDark;
        }
    }

    [ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color;
            }
            return default(Color);
        }
    }
    public class BrushToHexConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null) return null;
            string lowerHexString(int i) => i.ToString("X2").ToLower();
            var brush = (SolidColorBrush)value;
            var hex = lowerHexString(brush.Color.R) +
                      lowerHexString(brush.Color.G) +
                      lowerHexString(brush.Color.B);
            return "#" + hex;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
