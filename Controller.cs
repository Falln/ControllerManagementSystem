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

        public void setCheckedOut(Boolean isCheckedOut, string owner, string controllerStatus, string employeeID)
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
                //Add format: Name, ControllerType, Date, Time, In or Out, Owner, Status, and Initials/ID
                writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToShortTimeString(), isCheckedOut ? "Checked Out" : "Checked In", owner, controllerStatus, employeeID);
            }
        }

        public void checkOut(string owner, string controllerStatus, string employeeID)
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
                writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToShortTimeString(), "Checked Out", owner, controllerStatus, employeeID);
            }
        }

        public void checkIn(string owner, string controllerStatus, string employeeID)
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
                writer.WriteRow(name, controllerType.ToString(), now.ToShortDateString(), now.ToShortTimeString(), "Checked In", owner, controllerStatus, employeeID);
            }
        }

        public static ControllerType FromStringToControllerType(string controllerTypeString)
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
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Object is not a Controller or is null");
            }
        }
    }
}
