using System;
using System.IO;

namespace GPUPrefSwitcher
{
    /// <summary>
    /// Singleton abstraction of <see cref="AppLogger"/>
    /// </summary>
    public static class Logger
    {
        public static AppLogger inst 
        {
            get
            {
                if (inst_internal == null) throw new InvalidOperationException("Call this class's initialize function first");
                return inst_internal;
            }
            private set
            {
                inst_internal = value;
            }
        }

        private static AppLogger inst_internal; //using a class-level initializer causes stuff to not be written upon app start

        public static void Initialize(string logFolderPath_error, string logFileName_error, string logFolderPath_standard, string logFileName_standard, string archiveFolderPath)
        {
            inst_internal = AppLogger.Initialize(
                logFolderPath_error,
                logFileName_error, 
                logFolderPath_standard,
                logFileName_standard,
                archiveFolderPath
            );
        }
    }
}
