using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace GPUPrefSwitcher
{
    internal class SwitcherData
    {

        public SwitcherDataStruct CurrentSwitcherData; //to be read and modified from outside
        private SwitcherDataStruct prevSwitcherData; 

        private readonly string XML_PATH = Program.SavedDataPath + "Switcher.data";
        private XmlDocument xmlDocument = new XmlDocument();

        internal void ResetSwitcherDataFile()
        {
            var source = Path.Combine(Program.SavedDataPath, "defaults/Switcher.data");
            var destination = Path.Combine(Program.SavedDataPath, "Switcher.data");

            File.Copy(source, destination, true);

            //string message = "You will also need to restart the service, then re-open this app";
            //MessageBox.Show(message, "Service Restart Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //RestartService();

            //Close();
        }

        public static SwitcherData Initialize()
        {
            SwitcherData s = new();

            try
            {
                SwitcherDataStruct sDataStruct = s.LoadFromXML();
                s.CurrentSwitcherData = sDataStruct; //structs are copied on assignment
                s.prevSwitcherData = sDataStruct;

                return s;
            } catch (FileNotFoundException)
            {
                OverwriteSwitcherDataWithDefault();
            } catch (XmlException)
            {
                OverwriteSwitcherDataWithDefault();
            } catch (NullReferenceException)
            {
                OverwriteSwitcherDataWithDefault();
            }

            //the below should be reached only if an error is caught from above

            SwitcherDataStruct sDataStruct_retry = s.LoadFromXML();
            s.CurrentSwitcherData = sDataStruct_retry; //structs are copied on assignment
            s.prevSwitcherData = sDataStruct_retry;
            return s;

            void OverwriteSwitcherDataWithDefault()
            {
                Logger.inst.ErrorLog("Tried to recover from a Switcher.data error by overwriting with default");
                var source = Path.Combine(Program.SavedDataPath, "defaults/Switcher.data");
                var destination = Path.Combine(Program.SavedDataPath, "Switcher.data");
                File.Copy(source, destination, true);
            }
        }

        public void SaveToXML()
        {
            XmlNode root = xmlDocument.DocumentElement;

            XmlNode prevPowerLineStatus = root.SelectSingleNode("PreviousPowerLineStatus");
            prevPowerLineStatus.InnerText = CurrentSwitcherData.PrevPowerStatus_string.ToString().ToLower();

            XmlNode lastShutdownWasClean = root.SelectSingleNode("LastShutdownWasClean");
            lastShutdownWasClean.InnerText = CurrentSwitcherData.LastShutDownWasClean.ToString().ToLower();

            XmlNode dontShowErrorMessage = root.SelectSingleNode("DontShowErrorMessage");
            dontShowErrorMessage.InnerText = CurrentSwitcherData.DontShowErrorMessage.ToString().ToLower();

            prevSwitcherData = CurrentSwitcherData; //copy

            xmlDocument.Save(XML_PATH);
        }

        public bool ChangesArePending()
        {
            return !prevSwitcherData.Equals(CurrentSwitcherData);
        }

        public void RevertChanges()
        {
            CurrentSwitcherData = prevSwitcherData; //struct copy 
        }

        private SwitcherDataStruct LoadFromXML()
        {
            try
            {
                xmlDocument.Load(XML_PATH);
            } catch (Exception ex)
            {
                if(ex is XmlException)
                {
                    Logger.inst.ErrorLog($"Critical file {XML_PATH} is malformed. Fix the XML syntax error, or use the GUI to recreate this file.");
                    Logger.inst.ErrorLog($"{ex.Message}");
                }
                else if(ex is DirectoryNotFoundException)
                {
                    Logger.inst.ErrorLog($"Unable to find critical file {XML_PATH}. It is suggested to use the GUI to recreate this file.");
                }
                else
                {
                    Logger.inst.ErrorLog($"Unidentified error while trying to load critical app data from {XML_PATH}. It is suggested to use the GUI to recreate this file.");
                }

                throw;
            }
            

            XmlNode root = xmlDocument.DocumentElement;

            XmlNode prevPowerLineStatus = root.SelectSingleNode("PreviousPowerLineStatus");

            XmlNode lastShutdownWasClean = root.SelectSingleNode("LastShutdownWasClean");

            XmlNode dontShowErrorMessage = root.SelectSingleNode("DontShowErrorMessage");

            string prevPowerLineStatus_str = prevPowerLineStatus.InnerText;
            bool lastShutdownWasClean_bool = bool.Parse(lastShutdownWasClean.InnerText);
            bool dontShowErrorMessage_bool = bool.Parse(dontShowErrorMessage.InnerText);

            return new SwitcherDataStruct() { 
                PrevPowerStatus_string = prevPowerLineStatus_str,
                LastShutDownWasClean = lastShutdownWasClean_bool,
                DontShowErrorMessage = dontShowErrorMessage_bool
            };

        }
    }

    public struct SwitcherDataStruct
    {
        public required string PrevPowerStatus_string;
        public PowerLineStatus PrevPowerStatus_enum
        {
            get
            {
                if (PrevPowerStatus_string.ToLower() == "online")
                {
                    return PowerLineStatus.Online;
                } else if (PrevPowerStatus_string.ToLower() == "offline")
                {
                    return PowerLineStatus.Offline;
                } else
                {
                    throw new NotImplementedException();
                    return PowerLineStatus.Unknown;
                }
            } 
        }

        public required bool LastShutDownWasClean;

        public required bool DontShowErrorMessage;

        public override bool Equals(object obj)
        {
            return obj is SwitcherDataStruct @struct &&
                   PrevPowerStatus_string == @struct.PrevPowerStatus_string &&
                   LastShutDownWasClean == @struct.LastShutDownWasClean;
        }

        public override int GetHashCode()
        {
            int hashCode = -1611375530;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PrevPowerStatus_string);
            hashCode = hashCode * -1521134295 + LastShutDownWasClean.GetHashCode();
            return hashCode;
        }
    }
}
