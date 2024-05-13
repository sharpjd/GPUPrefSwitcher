using System;
using System.Windows.Forms;
using System.Xml;

namespace GPUPrefSwitcher
{

    public class AppOptions
    {
        private AppOptionsDataStruct prevOptions;
        public AppOptionsDataStruct CurrentOptions; 

        private readonly string XML_PATH = Program.SavedDataPath + "AppOptions.xml";
        XmlDocument xmlDocument = new XmlDocument();

        public void RevertToDefault_TopSection()
        {
            string defaultXmlPath = Program.SavedDataPath + "defaults/AppOptions.xml";

            AppOptionsDataStruct defaults = LoadFromXML(defaultXmlPath);

            CurrentOptions.SpoofPowerState = defaults.SpoofPowerState;
            CurrentOptions.SpoofPowerStateEnabled = defaults.SpoofPowerStateEnabled;
            CurrentOptions.UpdateInterval = defaults.UpdateInterval;
            CurrentOptions.EnableTips = defaults.EnableTips;
            CurrentOptions.EnableRealtimeLogging = defaults.EnableRealtimeLogging;
        }

        public AppOptions()
        {
            CurrentOptions = LoadFromXML(XML_PATH);
            prevOptions = CurrentOptions; //struct copy
        }

        public void SaveToXML(out bool requireServiceRestart)
        {
            requireServiceRestart = false;
            if(prevOptions.UpdateInterval != CurrentOptions.UpdateInterval) requireServiceRestart = true;
            if (prevOptions.SpoofPowerState != CurrentOptions.SpoofPowerState) requireServiceRestart = true;
            if (prevOptions.SpoofPowerStateEnabled != CurrentOptions.SpoofPowerStateEnabled) requireServiceRestart = true;
            if (prevOptions.RunTaskOnBattery != CurrentOptions.RunTaskOnBattery) requireServiceRestart = true;
            if (prevOptions.RunTaskPluggedIn != CurrentOptions.RunTaskPluggedIn) requireServiceRestart = true;
            if (prevOptions.EnableRealtimeLogging != CurrentOptions.EnableRealtimeLogging) requireServiceRestart = true;

            XmlNode enableTips = xmlDocument.DocumentElement.SelectSingleNode("EnableTips");
            enableTips.InnerText = CurrentOptions.EnableTips.ToString().ToLower();

            XmlNode updateInterval = xmlDocument.DocumentElement.SelectSingleNode("UpdateInterval");
            updateInterval.InnerText = CurrentOptions.UpdateInterval.ToString();

            XmlNode spoofPowerState = xmlDocument.DocumentElement.SelectSingleNode("SpoofPowerState");
            spoofPowerState.InnerText = PowerLineStatusToOfflineOrOnline(CurrentOptions.SpoofPowerState);
            spoofPowerState.Attributes["enabled"].Value = CurrentOptions.SpoofPowerStateEnabled.ToString().ToLower();

            XmlNode runTaskOnBattery = xmlDocument.DocumentElement.SelectSingleNode("RunTaskOnBattery");
            runTaskOnBattery.InnerText = CurrentOptions.RunTaskOnBattery.ToString().ToLower();

            XmlNode runTaskPluggedIn = xmlDocument.DocumentElement.SelectSingleNode("RunTaskPluggedIn");
            runTaskPluggedIn.InnerText = CurrentOptions.RunTaskPluggedIn.ToString().ToLower();

            XmlNode enableRealtimeLogging = xmlDocument.DocumentElement.SelectSingleNode("EnableRealtimeLogging");
            enableRealtimeLogging.InnerText = CurrentOptions.EnableRealtimeLogging.ToString().ToLower();

            prevOptions = CurrentOptions;//struct copy

            xmlDocument.Save(XML_PATH);
        }

        static Func<PowerLineStatus, string> PowerLineStatusToOfflineOrOnline = PowerLineStatusConversions.PowerLineStatusToOfflineOrOnline;
        static Func<string, PowerLineStatus> StringToPowerLineStatus = PowerLineStatusConversions.StringToPowerLineStatus;

        public void Reload()
        {
            prevOptions = LoadFromXML(XML_PATH);
            CurrentOptions = prevOptions;
        }

        private AppOptionsDataStruct LoadFromXML(string path)
        {
            try
            {
                xmlDocument.Load(path);

                XmlNode enableTips = xmlDocument.DocumentElement.SelectSingleNode("EnableTips");
                bool enableTips_ = bool.Parse(enableTips.InnerText);

                XmlNode updateInterval = xmlDocument.DocumentElement.SelectSingleNode("UpdateInterval");
                int updateInterval_ = int.Parse(updateInterval.InnerText);

                XmlNode spoofPowerState = xmlDocument.DocumentElement.SelectSingleNode("SpoofPowerState");
                string spoofPowerState_ = spoofPowerState.InnerText.ToLower();
                bool spoofPowerStateEnabled_ = bool.Parse(spoofPowerState.Attributes["enabled"].InnerText.ToLower());

                XmlNode runTaskOnBattery = xmlDocument.DocumentElement.SelectSingleNode("RunTaskOnBattery");
                bool runTaskOnBattery_bool = bool.Parse(runTaskOnBattery.InnerText);

                XmlNode runTaskPluggedIn = xmlDocument.DocumentElement.SelectSingleNode("RunTaskPluggedIn");
                bool runTaskPluggedIn_bool = bool.Parse(runTaskPluggedIn.InnerText);

                XmlNode enableRealtimeLogging = xmlDocument.DocumentElement.SelectSingleNode("EnableRealtimeLogging");
                bool enableRealtimeLogging_bool = bool.Parse(enableRealtimeLogging.InnerText);

                return new AppOptionsDataStruct
                {
                    UpdateInterval = updateInterval_,
                    SpoofPowerStateEnabled = spoofPowerStateEnabled_,
                    SpoofPowerState = StringToPowerLineStatus(spoofPowerState_),
                    EnableTips = enableTips_,
                    RunTaskOnBattery = runTaskOnBattery_bool,
                    RunTaskPluggedIn = runTaskPluggedIn_bool,
                    EnableRealtimeLogging = enableRealtimeLogging_bool,
                };
            } catch (XmlException)
            {
                Logger.inst.ErrorLog("AppOptions.xml is malformed, it is advised to fix the syntax or reset it from the app GUI");
                throw;
            } catch (Exception)
            {
                Logger.inst.ErrorLog("Generic error while loading data from AppOptions.xml, you can try resetting it from the app GUI");
                throw;
            }
        }

        public void RevertChanges()
        {
            CurrentOptions = prevOptions;//struct copy
        }

        public bool ChangesArePending()
        {
            //Debug.WriteLine(prevOptions);
            //Debug.WriteLine(CurrentOptions);
            return !prevOptions.Equals(CurrentOptions);
        }

    }


    public struct AppOptionsDataStruct
    {
        public required int UpdateInterval;
        public required bool SpoofPowerStateEnabled;
        public required PowerLineStatus SpoofPowerState;
        public required bool EnableTips;
        public required bool RunTaskOnBattery;
        public required bool RunTaskPluggedIn;
        public required bool EnableRealtimeLogging;
    }
}
