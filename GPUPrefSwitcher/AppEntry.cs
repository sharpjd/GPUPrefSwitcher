using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GPUPrefSwitcher
{
    /// <summary>
    /// Represents an App Entry in the Registry along with the user's preferences.
    /// </summary>
    public struct AppEntry
    {
        public required string AppPath { get; init; }

        /// <summary>
        /// Returns the filename if the value is not initialized.
        /// </summary>
        public string AppName
        {
            readonly get
            {
                if (string.IsNullOrEmpty(appName))
                {
                    return AppPath.Split('\\').Last();
                }
                return appName;
            }
            init
            {
                appName = value;
            }
        }
        private string appName;

        public required bool EnableSwitcher { get; init; }
        public required bool EnableFileSwapper { get; init; }
        public required string[] FileSwapperPaths { get; init; }
        public required PowerLineStatus[] SwapperStates { get; init; }//NOT part of GetHashCode; order should be 1:1 with FileSwapperPaths //TODO: we can probably include this without much trouble, GUI ought to reload before saving anyways
        public required int GPUPrefOnBattery { get; init; }
        public required int GPUPrefPluggedIn { get; init; }
        public required bool PendingAddToRegistry { get; init; }
        public required string RunOnBatteryPath { get; init; }
        public required string RunPluggedInPath { get; init; }
        public required bool SeenInRegistry { get; init; }
        public override readonly bool Equals(object obj)
        {
            return obj is AppEntry entry &&
                   AppPath == entry.AppPath &&
                   AppName == entry.AppName &&
                   //appName == entry.appName && //breaks for some reason; null comparison with empty string... let's just exclude this since we're not using it for now
                   EnableSwitcher == entry.EnableSwitcher &&
                   EnableFileSwapper == entry.EnableFileSwapper &&
                   FileSwapperPaths.SequenceEqual(entry.FileSwapperPaths) &&
                   GPUPrefOnBattery == entry.GPUPrefOnBattery &&
                   GPUPrefPluggedIn == entry.GPUPrefPluggedIn &&
                   RunOnBatteryPath == entry.RunOnBatteryPath &&
                   RunPluggedInPath == entry.RunPluggedInPath &&
                   PendingAddToRegistry == entry.PendingAddToRegistry;
        }

        //Equals() is much faster
        public override readonly int GetHashCode()
        {
            int hashCode = -985154422;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AppPath);
            //hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(appName); //see above comment for appName
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AppName);
            hashCode = hashCode * -1521134295 + EnableSwitcher.GetHashCode();
            hashCode = hashCode * -1521134295 + EnableFileSwapper.GetHashCode();
            hashCode = hashCode * -1521134295 + GetStringArrHash(FileSwapperPaths);
            hashCode = hashCode * -1521134295 + GetStringArrHash(FileSwapperPaths);
            hashCode = hashCode * -1521134295 + GPUPrefOnBattery.GetHashCode();
            hashCode = hashCode * -1521134295 + GPUPrefPluggedIn.GetHashCode();
            hashCode = hashCode * -1521134295 + RunOnBatteryPath.GetHashCode();
            hashCode = hashCode * -1521134295 + RunPluggedInPath.GetHashCode();
            hashCode = hashCode * -1521134295 + PendingAddToRegistry.GetHashCode();
            //TODO: need for AppName
            return hashCode;
        }
        
        public static int GetStringArrHash(string[] strings)
        {
            int hash = -335392656;
            for (int i = 0; i < strings.Length; i++)
            {
                hash = hash * -130699793 + strings[i].GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"AppEntry (enabled: {EnableSwitcher}; appname: {AppName}): {AppPath}");
            sb.AppendLine($"On Battery: {GPUPrefOnBattery}; Plugged in: On Battery: {GPUPrefPluggedIn}");
            sb.AppendLine($"File swapper (enabled: {EnableFileSwapper}):");
            for (int i = 0; i < FileSwapperPaths.Length; i++)
            {
                sb.AppendLine($"    SwapPath (state: {SwapperStates[i]}): {FileSwapperPaths[i]}");
            }
            return sb.ToString();
        }

        public static bool operator ==(AppEntry left, AppEntry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AppEntry left, AppEntry right)
        {
            return !(left == right);
        }

    }
}
