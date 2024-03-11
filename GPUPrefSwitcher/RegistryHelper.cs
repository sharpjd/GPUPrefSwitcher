using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace GPUPrefSwitcher
{

    /// <summary>
    /// Requires admin priveleges
    /// </summary>
    internal static class RegistryHelper
    {

        //this function might change a lot 
        public static void DebugReadRegValues()
        {
            var key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);

            Debug.WriteLine("SUBKEYS (0):");
            foreach (string s in key.GetSubKeyNames())
                System.Diagnostics.Debug.WriteLine(s);

            key = key.OpenSubKey("Software");
            Debug.WriteLine("SUBKEYS (1):");
            foreach (string s in key.GetSubKeyNames())
                System.Diagnostics.Debug.WriteLine(s);

            key = key.OpenSubKey("Microsoft");
            Debug.WriteLine("SUBKEYS (2):");
            foreach (string s in key.GetSubKeyNames())
                System.Diagnostics.Debug.WriteLine(s);

            key = key.OpenSubKey("DirectX");
            Debug.WriteLine("SUBKEYS (3):");
            foreach (string s in key.GetSubKeyNames())
                System.Diagnostics.Debug.WriteLine(s);

            key = key.OpenSubKey("UserGpuPreferences");
            Debug.WriteLine("SUBKEYS (4):");
            foreach (string s in key.GetSubKeyNames())
                System.Diagnostics.Debug.WriteLine(s);

            Debug.WriteLine("VALUES:");
            foreach (string s in key.GetValueNames())
                System.Diagnostics.Debug.WriteLine(s);
            
        }

        public static readonly string[] ignoreValues = new string[]
        {
            "DirectXUserGlobalSettings" //TODO really hide this? Could become part of the setup
        };

        public static IEnumerable<string> GetGpuPrefPathvalueNames()
        {
            var key = GetLoggedInGpuPrefKey();
            return key.GetValueNames();
        }
        
        private static string GetGpuPrefPathvalueData(string pathvalue)
        {
            var key = GetLoggedInGpuPrefKey();
            return key.GetValue(pathvalue)?.ToString();
        }

        public static bool GpuPrefPathvalueExistsAndIsValid(string pathvalue)
        {
            var key = GetLoggedInGpuPrefKey();
            bool exists = key.GetValueNames().Contains(pathvalue);
            if (!exists) return false;

            string data = key.GetValue(pathvalue)?.ToString();

            Match match = Regex.Match(data, @"GpuPreference=(\d+)");

            if(!match.Success) return false;

            return true;

        }

        /// <summary>
        /// Adds a Registry key with a Name/path and GPU preference value to the part of the registry containing the system graphics preferences.
        /// Throws <see cref="ArgumentException"/> if the Registry value name already exists.
        /// </summary>
        /// <param name="pathvalue"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        internal static void AddGpuPref(string pathvalue, int value)
        {

            string valueName = pathvalue;
            string valueData = $"GpuPreference={value};";

            if (GetLoggedInGpuPrefKey().GetValueNames().Contains(pathvalue))
            {
                //necessary, otherwise we will overwrite the data 
                throw new ArgumentException($"registry already already contains the value (i.e. path) {pathvalue}");
            }

            GetLoggedInGpuPrefKey().SetValue(valueName, valueData);

        }

        /// <summary>
        /// Changes the GPU preference value of the key with the specified Value Name/path. 
        /// </summary>
        /// <param name="pathvalue"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NoGPUPreferenceDataException"></exception>
        public static void SetGpuPref(string pathvalue, int value)
        {
            string data = GetGpuPrefPathvalueData(pathvalue);

            if(data == null) throw new ArgumentException("pathvalue " + pathvalue + " returned null; it probably doesn't exist in the registry");
            if(value < 0) throw new ArgumentException("shouldn't be setting GPUPreference value to something negative (tried setting value to " + value + ")");

            Match match = Regex.Match(data, @"GpuPreference=(\d+)");
            if(!match.Success)
            {
                throw new NoGPUPreferenceDataException("SetGpuPref(): pathvalue " + pathvalue + " exists but has no valid GPUPreference value");
            }

            string modified = Regex.Replace(data, @"GpuPreference=(\d+)", "GpuPreference="+value).ToString();

            SetGpuPref_internal(pathvalue, modified);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathvalue"></param>
        /// <returns>The value of the GpuPreference data entry of the specified value.</returns>
        /// <exception cref="ArgumentException">if the pathvalue does not exist in the entry</exception>
        /// <exception cref="NoGPUPreferenceDataException">if the pathvalue exists but it lacks a "GpuPreference=" data entry</exception>
        public static int GetGpuPrefValue(string pathvalue)
        {
            string data = GetGpuPrefPathvalueData(pathvalue);

            if (data == null) throw new ArgumentException("pathvalue " + pathvalue + " does not exist in the registry");

            int prefValue;
            Match match = Regex.Match(data, @"GpuPreference=(\d+)");

            /*
            if (match.Success == false) throw new InvalidOperationException("pathvalue " + pathvalue + " exists but " +
                "has no GpuPreference in its data; exclude calling this entry from the algorithm");
            */

            if (match.Success == false)
            {
                /*
                Debug.WriteLine("Warning: pathvalue \"" + pathvalue + "\" exists in the registry but lacks a GpuPreference data value, returning -1");
                return -1;
                */
                throw new NoGPUPreferenceDataException("GetGpuPrefValue(): pathvalue " + pathvalue + " exists but has no valid GPUPreference value");
            }
                

            prefValue = int.Parse(match.Groups[1].Value);

            return prefValue;
        }

        private static void SetGpuPref_internal(string pathvalue, string data)
        {

            if (!GpuPrefPathvalueExistsAndIsValid(pathvalue)) throw new ArgumentException("pathvalue " + pathvalue + " does not exist in the registry or doesn't have a GpuPreference data entry");

            RegistryKey gpuPrefKey = GetLoggedInGpuPrefKey();

            Logger.inst.Log($"Modifiying registry key: setting {pathvalue} to {data}");

            gpuPrefKey.SetValue(pathvalue, data);
        }


        private static readonly string registryPath = @"\Software\Microsoft\DirectX\UserGpuPreferences";
        /*
         * This is more complicated than it looks; OpenSubKey(RegistryHive.CurrentUser) will not return what is seen in HKEY_CURENT_USER in Registry Viewer; 
         * what you're actually seeing is the directory of the logged-in user under HKEY_USERS, masquerading as HKEY_CURRENT_USER; so you actually need 
         * to grab the correct one from under the HKEY_USERS directory, which is what this function does
         */
        /// <summary>
        /// See adjacent block comment for more details.
        /// </summary>
        /// <returns>The root RegistryKey of the current logged in user, or null if not found.</returns>
        private static RegistryKey GetLoggedInGpuPrefKey()
        {

            string user = GetLoggedOnUserSID();
            Logger.inst.Log($"GetLoggedInGpuPrefPath: Getting logged in user: {user}", 2000);
            if (user == null || user == string.Empty) return null;

            var hklmUsers = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64);
            var hklmCurrentUserActual = hklmUsers.OpenSubKey(user + registryPath, true);
            
            return hklmCurrentUserActual;
        }
        

        enum TokenInformationClass
        {
            TokenOwner = 4,
        }

        struct TokenOwner
        {
            public IntPtr Owner;
        }

        [DllImport("advapi32.dll", EntryPoint = "GetTokenInformation", SetLastError = true)]
        static extern bool GetTokenInformation(
            IntPtr tokenHandle,
            TokenInformationClass tokenInformationClass,
            IntPtr tokenInformation,
            int tokenInformationLength,
            out int ReturnLength);

        [DllImport("kernel32.dll")]
        private static extern UInt32 WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool ConvertSidToStringSid(IntPtr sid, [In, Out, MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid);

        /*
         * Source of code: https://stackoverflow.com/a/39265967
         */
        public static string GetLoggedOnUserSID()
        {
            IntPtr tokenOwnerPtr;
            int tokenSize;
            IntPtr hToken;

            // Get a token from the logged on session
            // !!! this line will only work within the SYSTEM session !!!
            WTSQueryUserToken(WTSGetActiveConsoleSessionId(), out hToken);

            // Get the size required to host a SID
            GetTokenInformation(hToken, TokenInformationClass.TokenOwner, IntPtr.Zero, 0, out tokenSize);
            tokenOwnerPtr = Marshal.AllocHGlobal(tokenSize);

            // Get the SID structure within the TokenOwner class
            GetTokenInformation(hToken, TokenInformationClass.TokenOwner, tokenOwnerPtr, tokenSize, out tokenSize);
            TokenOwner tokenOwner = (TokenOwner)Marshal.PtrToStructure(tokenOwnerPtr, typeof(TokenOwner));

            // Convert the SID into a string
            string strSID = "";
            ConvertSidToStringSid(tokenOwner.Owner, ref strSID);
            Marshal.FreeHGlobal(tokenOwnerPtr);

            if (strSID == null || strSID == string.Empty) throw new InvalidOperationException("Could not get the SID of the currently logged in user (is there any logged in user at all?)");
            return strSID;
        }

        /// <summary>
        /// Throw this when a registry pathvalue exists but it has no "GPUPreference=" entry. This exists because it's a very specific
        /// circumstance, so don't abuse it
        /// </summary>
        public class NoGPUPreferenceDataException : InvalidOperationException
        {
            public NoGPUPreferenceDataException(string message) : base(message) { }
        }

    }
}
