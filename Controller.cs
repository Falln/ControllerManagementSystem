using Sky.Data.Csv;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerManagementSystem
{
    public class Controller : IComparable
    {
        //TODO if adding more Controller types, add things here
        public enum ControllerType
        {
            JoyCon,
            ProController,
            SwitchConsole,
            XboxWireless,
            XboxWired,
            Mouse,
            Other
        }

        public readonly ControllerType controllerType;
        public readonly string name;
        public string controllerStatus;
        public string currentOwner;
        public Boolean isCheckedOut = false;
        public string historyFile;

        public int totalEntriesToSave = Properties.Settings.Default.totalEntriesToSave;

        public Controller()
        {
            controllerType = ControllerType.Other;
            name = "";
            controllerStatus = "";
            historyFile = "";
            currentOwner = Properties.Settings.Default.defaultOwner;
        }
        public Controller(string name, ControllerType controllerType)
        {
            this.name = name;
            this.controllerType = controllerType;
            this.controllerStatus = "New";
            historyFile = "Controller CSV Files/" + name + " History CSV.csv";
            currentOwner = Properties.Settings.Default.defaultOwner;
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
                        writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToLongTimeString(), "Checked In", currentOwner, controllerStatus, "");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }
        public Controller(string name, ControllerType controllerType, string controllerStatus)
        {
            this.name = name;
            this.controllerType = controllerType;
            this.controllerStatus = controllerStatus;
            historyFile = "Controller CSV Files/" + name + " History CSV.csv";
            currentOwner = Properties.Settings.Default.defaultOwner;
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
                        writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToLongTimeString(), "Checked In", currentOwner, controllerStatus, "");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }
        public Controller(string name, ControllerType controllerType, Boolean isCheckedOut, string controllerStatus, string owner)
        {
            this.name = name;
            this.controllerType = controllerType;
            this.controllerStatus = controllerStatus;
            this.isCheckedOut = isCheckedOut;
            this.currentOwner = owner;
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
                        writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToLongTimeString(), "Checked In", currentOwner, controllerStatus, "");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        public void setCheckedOut(Boolean isCheckedOut, string owner, string controllerStatus, string employeeID)
        {
            //Check if the new checkout status matches the current checkout status. If so, do nothing
            if (this.isCheckedOut == isCheckedOut)
            {
                return;
            }
            //Set the controller to checked out
            this.isCheckedOut = isCheckedOut;
            if (isCheckedOut)
                currentOwner = owner;

            //Add a list for the reader to unload on
            List<List<string>> controllerCSVList;

            //Start the CSV writer
            var dataResolver = new BaseCSVResolver();
            using (var reader = CsvReader<List<string>>.Create(historyFile, dataResolver))
            {
                controllerCSVList = reader.ToList();
            }

            //If there are less entries than the total allowed, then add one to the end
            if (controllerCSVList.Count < totalEntriesToSave)
            {
                CsvWriterSettings writerSettings = new();
                writerSettings.AppendExisting = true;
                using (var writer = CsvWriter.Create(historyFile, writerSettings))
                {
                    DateTime now = DateTime.Now;
                    //Add a CSV entry with the checked out status as the one given
                    //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials/ID
                    writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToLongTimeString(), isCheckedOut ? "Checked Out" : "Checked In", owner, controllerStatus, employeeID);
                    writer.Close();
                }
            }
            //If there are more entries than the total allowed, then cycle entries down one and add the new one to the end
            else
            {
                CsvWriterSettings writerSettings = new();
                writerSettings.AppendExisting = false;
                writerSettings.OverwriteExisting = true;
                using (var writer = CsvWriter.Create(historyFile, writerSettings))
                {
                    for (int k = 1; k <= totalEntriesToSave - 1; k++)
                    {
                        writer.WriteRow(controllerCSVList.ElementAt(k));
                    }
                    DateTime now = DateTime.Now;
                    //Add a CSV entry with the checked out status as the one given
                    //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials/ID
                    writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToLongTimeString(), isCheckedOut ? "Checked Out" : "Checked In", owner, controllerStatus, employeeID);
                    writer.Close();
                }
            }
        }

        public void checkOut(string owner, string controllerStatus, string employeeID)
        {
            //Check if the controller is already checked out, if so, do nothing
            if (this.isCheckedOut)
            {
                return;
            }

            //Set the controller to checked out and set the owner
            this.isCheckedOut = true;
            this.currentOwner = owner;

            //Add a list for the reader to unload on
            List<List<string>> controllerCSVList;

            //Start the CSV writer
            var dataResolver = new BaseCSVResolver();
            using (var reader = CsvReader<List<string>>.Create(historyFile, dataResolver))
            {
                controllerCSVList = reader.ToList();
            }

            //If there are less entries than the total allowed, then add one to the end
            if (controllerCSVList.Count < totalEntriesToSave)
            {
                CsvWriterSettings writerSettings = new();
                writerSettings.AppendExisting = true;
                using (var writer = CsvWriter.Create(historyFile, writerSettings))
                {
                    DateTime now = DateTime.Now;
                    //Add a CSV entry of "checked out"
                    //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                    writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToLongTimeString(), "Checked Out", owner, controllerStatus, employeeID);
                    writer.Close();
                }
            }
            //If there are more entries than the total allowed, then cycle entries down one and add the new one to the end
            else
            {
                CsvWriterSettings writerSettings = new();
                writerSettings.AppendExisting = false;
                writerSettings.OverwriteExisting = true;
                using (var writer = CsvWriter.Create(historyFile, writerSettings))
                {
                    for (int k = 1; k <= totalEntriesToSave - 1; k++)
                    {
                        writer.WriteRow(controllerCSVList.ElementAt(k));
                    }
                    DateTime now = DateTime.Now;
                    //Add a CSV entry of "checked out"
                    //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                    writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToLongTimeString(), "Checked Out", owner, controllerStatus, employeeID);
                    writer.Close();
                }
            }
        }

        public void checkIn(string controllerStatus, string employeeID)
        {
            //Check if the controller is already checked in, if so, do nothing
            if (!this.isCheckedOut)
            {
                return;
            }

            //Set the controller to checked in
            this.isCheckedOut = false;
            string owner = Properties.Settings.Default.defaultOwner;
            this.currentOwner = owner;

            //Add a list for the reader to unload on
            List<List<string>> controllerCSVList;

            //Start the CSV writer
            var dataResolver = new BaseCSVResolver();
            using (var reader = CsvReader<List<string>>.Create(historyFile, dataResolver))
            {
                controllerCSVList = reader.ToList();
            }

            //If there are less entries than the total allowed, then add one to the end
            if (controllerCSVList.Count < totalEntriesToSave)
            {
                CsvWriterSettings writerSettings = new();
                writerSettings.AppendExisting = true;
                using (var writer = CsvWriter.Create(historyFile, writerSettings))
                {
                    DateTime now = DateTime.Now;
                    //Add a CSV entry of "checked in"
                    //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                    writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToLongTimeString(), "Checked In", owner, controllerStatus, employeeID);
                    writer.Close();
                }
            }
            //If there are more entries than the total allowed, then cycle entries down one and add the new one to the end
            else
            {
                CsvWriterSettings writerSettings = new();
                writerSettings.AppendExisting = false;
                writerSettings.OverwriteExisting = true;
                using (var writer = CsvWriter.Create(historyFile, writerSettings))
                {
                    for (int k = 1; k <= totalEntriesToSave - 1; k++)
                    {
                        writer.WriteRow(controllerCSVList.ElementAt(k));
                    }
                    DateTime now = DateTime.Now;
                    //Add a CSV entry of "checked in"
                    //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials
                    writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToLongTimeString(), "Checked In", owner, controllerStatus, employeeID);
                    writer.Close();
                }
            }
        }

        public List<HistoryEntry> GetControllerHistory()
        {
            List<HistoryEntry> controllerHistoryFromCSVs = new List<HistoryEntry>();
            //Start the CSV writer
            var dataResolver = new ControllerHistroyResolver();
            using (var reader = CsvReader<HistoryEntry>.Create(historyFile, dataResolver))
            {
                foreach (var controllerHistoryLine in reader)
                {
                    controllerHistoryFromCSVs.Add(controllerHistoryLine);
                }
            }
            return controllerHistoryFromCSVs;
        }

        public static Boolean ContainsSameName(List<Controller> controllerList, string newName)
        {
            foreach (Controller controller in controllerList)
            {
                if (controller.name.Equals(newName))
                {
                    return true;
                }
            }
            return false;
        }

        //TODO if adding more Controller types, add things here
        public static ControllerType FromStringToControllerType(string controllerTypeString)
        {
            switch (controllerTypeString)
            {
                case "JoyCon":
                    return ControllerType.JoyCon;
                case "ProController":
                    return ControllerType.ProController;
                case "SwitchConsole":
                    return ControllerType.SwitchConsole;
                case "XboxWireless":
                    return ControllerType.XboxWireless;
                case "XboxWired":
                    return ControllerType.XboxWired;
                case "Mouse":
                    return ControllerType.Mouse;
                case "Other":
                    return ControllerType.Other;
                default:
                    return ControllerType.Other;
            }
        }

        public int CompareTo(Controller obj)
        {
            //Check if there's a number, if there's not, save it as -1
            string thisNameNumbers = new string(this.name.Where(Char.IsDigit).ToArray());
            double thisControllerNum = -1;
            if (!(thisNameNumbers.Length == 0))
            {
                thisControllerNum = Double.Parse(thisNameNumbers);
            }

            //Check if there's a number, if there's not, save it as -1
            string newNameNumbers = new string(obj.name.Where(Char.IsDigit).ToArray());
            double newControllerNum = -1;
            if (!(newNameNumbers.Length == 0))
            {
                newControllerNum = Double.Parse(newNameNumbers);
            }

            //First compare length, then check if both have a number, then compare numbers, then assume the same
            if (this.name.Length < obj.name.Length)
                return -1;
            else if (this.name.Length > obj.name.Length)
                return 1;
            else if (thisControllerNum == -1 && newControllerNum != -1)
                return 1;
            else if (thisControllerNum != -1 && newControllerNum == -1)
                return -1;
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
            }
            catch (Exception _)
            {
                throw new ArgumentException("Object is not a Controller or is null");
            }
        }

        public class HistoryEntry
        {
            public ControllerType controllerType { get; set; }
            public string name { get; set; }
            public string condition { get; set; }
            public string currentOwner { get; set; }
            public string checkedStatus { get; set; }
            public DateTime dateTime { get; set; }
            public string initials { get; set; }

        }
    }
}
