using System;
using System.Windows.Forms;
using System.Xml;

namespace GPUPrefSwitcher
{
    [Obsolete]
    internal class SwitcherOptionsXML
    {
        private readonly string XML_PREFERENCES_PATH = Program.SavedDataPath + "AppOptions.xml";

        XmlDocument xmlDocument = new XmlDocument();

        public SwitcherOptionsXML()
        {
            Initialize();
        }

        private void Initialize()
        {
            xmlDocument.Load(XML_PREFERENCES_PATH);
        }

        private void ReloadXML()
        {
            xmlDocument.Load(XML_PREFERENCES_PATH);
        }

        internal int GetUpdateInterval()
        {
            XmlNode updateInterval = xmlDocument.DocumentElement.SelectSingleNode("UpdateInterval");
            return int.Parse(updateInterval.InnerText);
        }
        internal void SetUpdateInterval(int intervalMillis)
        {
            ReloadXML();
            SetNodeValueInnertextAndSave(xmlDocument.DocumentElement.SelectSingleNode("UpdateInterval"), intervalMillis.ToString());
        }

        internal bool GetEnableTips()
        {
            XmlNode enableTips = xmlDocument.DocumentElement.SelectSingleNode("EnableTips");
            return bool.Parse(enableTips.InnerText);
        }
        internal void SetEnableTips(bool enabled)
        {
            ReloadXML();
            SetNodeValueInnertextAndSave(xmlDocument.DocumentElement.SelectSingleNode("EnableTips"), enabled.ToString().ToLower());
        }

        internal bool GetSpoofPowerStateEnabled()
        {
            XmlNode spoofPowerState = xmlDocument.DocumentElement.SelectSingleNode("SpoofPowerState");
            return bool.Parse(spoofPowerState.Attributes["enabled"].Value);
        }
        internal void SetSpoofPowerStateEnabled(bool enabled)
        {
            ReloadXML(); //NECESSARY otherwise we might save an old copy
            XmlNode spoofPowerState = xmlDocument.DocumentElement.SelectSingleNode("SpoofPowerState");
            spoofPowerState.Attributes["enabled"].Value = enabled.ToString();
            xmlDocument.Save(XML_PREFERENCES_PATH);
        }

        internal PowerLineStatus GetSpoofPowerState()
        {
            XmlNode spoofPowerState = xmlDocument.DocumentElement.SelectSingleNode("SpoofPowerState");
            string state = spoofPowerState.InnerText.ToLower();
            if (state == "offline")
            {
                return PowerLineStatus.Offline;
            } else if (state == "online")
            {
                return PowerLineStatus.Online;
            } else
            {
                throw new Exception();
            }
        }
        internal void SetSpoofPowerState(string state)
        {
            ReloadXML();
            SetNodeValueInnertextAndSave(xmlDocument.DocumentElement.SelectSingleNode("SpoofPowerState"), state);
            //xmlDocument.Save(XML_PREFERENCES_PATH);
            /*
            string state;
            if (powerLineStatus == PowerLineStatus.Offline)
            {
                state = "offline";
            }
            else if (powerLineStatus == PowerLineStatus.Online)
            {
                state = "online";
            }
            else
            {
                throw new Exception();
            }

            SetNodeValueAndSave(xmlDocument.DocumentElement.SelectSingleNode("SpoofPowerState"), state);
            */
        }

        public PowerLineStatus GetPrevPowerLineStatus()
        {
            XmlNode prevPowerLineStatus = xmlDocument.DocumentElement.SelectSingleNode("PreviousPowerLineStatus");
            string s = prevPowerLineStatus.InnerText.ToLower();
            if (s == "online")
            {
                return PowerLineStatus.Online;
            } else if (s == "offline")
            {
                return PowerLineStatus.Offline;
            } else
            {
                throw new XmlException(); //placeholder
            }
        }
        internal void SetPrevPowerLineStatus(PowerLineStatus status)
        {
            ReloadXML();
            if (status == PowerLineStatus.Online)
            {
                SetNodeValueInnertextAndSave(xmlDocument.DocumentElement.SelectSingleNode("PreviousPowerLineStatus"), "online");
            }
            else if (status == PowerLineStatus.Offline)
            {
                SetNodeValueInnertextAndSave(xmlDocument.DocumentElement.SelectSingleNode("PreviousPowerLineStatus"), "offline");
            }
            else
            {
                throw new ArgumentException();
            }
            
        }

        void SetNodeValueInnertextAndSave(XmlNode xmlNode, string innerText)
        {
            throw new NotImplementedException();
        }

    }
}

/*
public struct SwitcherOptionsUIData
{
    public int UpdateInterval;
    public bool SpoofPowerStateEnabled;
    public string SpoofPowerState;
    public bool EnableTips;

    //does not contain PrevPowerLineStatus nor LastShutdownWasClean because this is used for the UI
    public SwitcherOptionsUIData(int updateInterval, bool spoofPowerStateEnabled, string spoofPowerState, bool enableTips)
    {
        UpdateInterval = updateInterval;
        SpoofPowerStateEnabled = spoofPowerStateEnabled;
        SpoofPowerState = spoofPowerState;
        EnableTips = enableTips;
    }

    public override bool Equals(object obj)
    {
        return obj is SwitcherOptionsUIData data &&
               UpdateInterval == data.UpdateInterval &&
               SpoofPowerStateEnabled == data.SpoofPowerStateEnabled &&
               SpoofPowerState == data.SpoofPowerState &&
               EnableTips == data.EnableTips;
    }

    public override int GetHashCode()
    {
        int hashCode = 1234941106;
        hashCode = hashCode * -1521134295 + UpdateInterval.GetHashCode();
        hashCode = hashCode * -1521134295 + SpoofPowerStateEnabled.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SpoofPowerState);
        hashCode = hashCode * -1521134295 + EnableTips.GetHashCode();
        return hashCode;
    }
*/
