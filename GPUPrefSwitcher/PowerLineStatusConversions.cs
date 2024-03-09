using System;
using System.Windows.Forms;

namespace GPUPrefSwitcher
{
    public static class PowerLineStatusConversions
    {

        public static string PowerLineStatusToOfflineOrOnline(PowerLineStatus powerLineStatus)
        {
            if (powerLineStatus == PowerLineStatus.Offline)
            {
                return "offline";
            }
            else if (powerLineStatus == PowerLineStatus.Online)
            {
                return "online";
            }
            else
            {
                throw new ArgumentException("unknown powerlinestatus " + powerLineStatus);
            }
        }

        public static PowerLineStatus[] StringArrToPowerLineStatusArr(string[] strings)
        {
            PowerLineStatus[] powerLineStatuses = new PowerLineStatus[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                powerLineStatuses[i] = StringToPowerLineStatus(strings[i]);
            }

            return powerLineStatuses;
        }

        public static PowerLineStatus StringToPowerLineStatus(string str)
        {
            string s = str.ToLower();
            if (s == "offline")
            {
                return PowerLineStatus.Offline;
            }
            else if (s == "online")
            {
                return PowerLineStatus.Online;
            }
            else
            {
                throw new ArgumentException($"unknown string for conversion: {s}");
            }
        }
    }
}